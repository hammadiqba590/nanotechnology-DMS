using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.PspCategory;
using NanoDMSAdminService.DTO.PspCurrency;
using NanoDMSAdminService.DTO.PspDocument;
using NanoDMSAdminService.Filters;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Services.Interfaces;
using NanoDMSAdminService.UnitOfWorks;
using NanoDMSSharedLibrary.CacheKeys;
using System.Text.Json;

namespace NanoDMSAdminService.Services.Implementations
{
    public class PspCurrencyService : IPspCurrencyService
    {
        private readonly IUnitOfWork _uow;
        private readonly IDistributedCache _cache;

        public PspCurrencyService(IUnitOfWork uow, IDistributedCache cache)
        {
            _uow = uow;
            _cache = cache;
        }

       
        public async Task<IEnumerable<PspCurrencyDto>> GetAllAsync()
        {
            const string cacheKey = "pspcurrencies:all";

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<IEnumerable<PspCurrencyDto>>(cached)!;

            var pspCurrency = await _uow.PspCurrencies.GetAllByConditionAsync(b =>
                !b.Deleted && b.Is_Active
            );

            var result = pspCurrency.Select(x => new PspCurrencyDto
            {
                Id = x.Id,
                Psp_Id = x.Psp_Id,
                Currency_Id = x.Currency_Id,
                Is_Active = x.Is_Active,
                Deleted = x.Deleted,
                Published = x.Published,
                Business_Id = x.Business_Id,
                BusinessLocation_Id = x.BusinessLocation_Id,
                Create_Date = x.Create_Date,
                Create_User = x.Create_User,
                Last_Update_Date = x.Last_Update_Date,
                Last_Update_User = x.Last_Update_User,
                RecordStatus = x.RecordStatus
            });

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) });

            return result;
        }

        public async Task<PspCurrencyDto?> GetByIdAsync(Guid id)
        {
            var cacheKey = $"pspcurrencies:{id}";

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<PspCurrencyDto>(cached);

            var pspCurrency = await _uow.PspCurrencies.GetByIdAsync(id);
            var dto = pspCurrency == null ? null : MapToDto(pspCurrency);

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dto), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15) });

            return dto;
        }

        public async Task<PaginatedResponseDto<PspCurrencyDto>> GetPagedAsync(PspCurrencyFilterModel filter)
        {
            var cacheKey = PspCurrencyCacheKeys.Paged(filter.PageNumber, 
                filter.PageSize,
                filter.Psp_Id?.ToString() ?? string.Empty,
                filter.Currency_Id?.ToString() ?? string.Empty
                );

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<PaginatedResponseDto<PspCurrencyDto>>(cached)!;

            var query = _uow.PspCurrencies.GetQueryable();

            if (filter.Psp_Id.HasValue)
                query = query.Where(x => x.Psp_Id == filter.Psp_Id);

            if (filter.Currency_Id.HasValue)
                query = query.Where(x => x.Currency_Id == filter.Currency_Id);

            query = query.OrderByDescending(x => x.Create_Date);

            var totalRecords = await query.CountAsync();

            var pspCurrency = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var result = new PaginatedResponseDto<PspCurrencyDto>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / filter.PageSize),
                Data = pspCurrency.Select(MapToDto).ToList()
            };

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15) });

            return result;
        }

        public async Task<PspCurrencyDto> CreateAsync(PspCurrencyCreateDto dto, string userId)
        {
            var pspCurrency = new PspCurrency
            {
                Id = Guid.NewGuid(),
                Psp_Id = dto.Psp_Id,
                Currency_Id = dto.Currency_Id,
                BusinessLocation_Id = dto.Business_Location_Id,
                Business_Id = dto.Business_Id,
                Is_Active = true,
                Deleted = false,
                Published = true,
                RecordStatus = Blocks.RecordStatus.Active,
                Create_Date = DateTime.UtcNow,
                Create_User = Guid.Parse(userId)
            };

            await _uow.PspCurrencies.AddAsync(pspCurrency);
            await _uow.SaveAsync();

            // 🔥 CACHE INVALIDATION
            await _cache.RemoveAsync(PspCurrencyCacheKeys.All);

            return MapToDto(pspCurrency);
        }

        public async Task<PspCurrencyDto> UpdateAsync(Guid id, PspCurrencyUpdateDto dto, string userId)
        {
            var entity = await _uow.PspCurrencies.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Psp Currency not found");

            entity.Psp_Id = dto.Psp_Id;
            entity.Currency_Id = dto.Currency_Id;
            

            entity.Last_Update_Date = DateTime.UtcNow;
            entity.Last_Update_User = Guid.Parse(userId);

            _uow.PspCurrencies.Update(entity);
            await _uow.SaveAsync();

            // 🔥 CACHE INVALIDATION
            await _cache.RemoveAsync(PspCurrencyCacheKeys.All);
            await _cache.RemoveAsync(PspCurrencyCacheKeys.ById(id));

            return MapToDto(entity);
        }

        public async Task<PspCurrencyDto> DeleteAsync(Guid id, string userId)
        {
            var pspCurrency = await _uow.PspCurrencies.GetByIdAsync(id);
            if (pspCurrency == null) return new PspCurrencyDto();

            pspCurrency.Deleted = true;
            pspCurrency.Published = false;
            pspCurrency.Is_Active = false;
            pspCurrency.RecordStatus = Blocks.RecordStatus.Inactive;
            pspCurrency.Last_Update_Date = DateTime.UtcNow;
            pspCurrency.Last_Update_User = Guid.Parse(userId);

            _uow.PspCurrencies.Update(pspCurrency);
            await _uow.SaveAsync();

            // 🔥 CACHE INVALIDATION
            await _cache.RemoveAsync(PspCurrencyCacheKeys.All);
            await _cache.RemoveAsync(PspCurrencyCacheKeys.ById(id));

            return MapToDto(pspCurrency);
        }
        private static PspCurrencyDto MapToDto(PspCurrency x) => new()
        {
            Id = x.Id,
            Psp_Id = x.Psp_Id,
            Psp_Name = x.Psp.Name,
            Currency_Id = x.Currency_Id,
            Currency_Name = x.Currency.Name,
            Is_Active = x.Is_Active,
            Deleted = x.Deleted,
            Published = x.Published,
            Business_Id = x.Business_Id,
            BusinessLocation_Id = x.BusinessLocation_Id,
            Create_Date = x.Create_Date,
            Create_User = x.Create_User,
            Last_Update_Date = x.Last_Update_Date,
            Last_Update_User = x.Last_Update_User,
            RecordStatus = x.RecordStatus
        };
    }
}
