using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.Country;
using NanoDMSAdminService.DTO.Currency;
using NanoDMSAdminService.Filters;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Services.Interfaces;
using NanoDMSAdminService.UnitOfWorks;
using NanoDMSSharedLibrary.CacheKeys;
using System.Text.Json;

namespace NanoDMSAdminService.Services.Implementations
{
    public class CurrencyService : ICurrencyService
    {
        private readonly IUnitOfWork _uow;
        private readonly IDistributedCache _cache;

        public CurrencyService(IUnitOfWork uow, IDistributedCache cache)
        {
            _uow = uow;
            _cache = cache;
        }

        public async Task<IEnumerable<CurrencyDto>> GetAllAsync()
        {
            const string cacheKey = "currencies:all";

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<IEnumerable<CurrencyDto>>(cached)!;

            var currencies = await _uow.Currencies.GetAllByConditionAsync(b => !b.Deleted && b.Is_Active);

            var result = currencies.Select(b => new CurrencyDto
            {
                Id = b.Id,
                Name = b.Name,
                Code = b.Code,
                Symbol = b.Symbol,
                Country_Id = b.Country_Id,
                Country_Name = b.Country != null ? b.Country.Name : "",
                Deleted = b.Deleted,
                Published = b.Published,
                Create_Date = b.Create_Date,
                Create_User = b.Create_User,
                Last_Update_Date = b.Last_Update_Date,
                Last_Update_User = b.Last_Update_User,
                Business_Id = b.Business_Id,
                BusinessLocation_Id = b.BusinessLocation_Id,
                Is_Active = b.Is_Active,
                RecordStatus = b.RecordStatus,
            });

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) });

            return result;
        }
        public async Task<CurrencyDto?> GetByIdAsync(Guid id)
        {
            var cacheKey = $"currencies:{id}";

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<CurrencyDto>(cached);

            var currency = await _uow.Currencies.GetByIdAsync(id);
            if (currency == null) return null;

            var dto = new CurrencyDto
            {
                Id = currency.Id,
                Code = currency.Code,
                Name = currency.Name,
                Symbol = currency.Symbol,
                Country_Id = currency.Country_Id,
                Country_Name = currency.Country?.Name,
                Deleted = currency.Deleted,
                Published = currency.Published,
                Is_Active = currency.Is_Active,
                Create_Date = currency.Create_Date,
                Create_User = currency.Create_User,
                Last_Update_User = currency.Last_Update_User,
                Last_Update_Date = currency.Last_Update_Date
            };

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dto), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)});

            return dto;
        }
        public async Task<PaginatedResponseDto<CurrencyDto>> GetPagedAsync(CurrencyFilterModel filter)
        {
            var cacheKey = CurrencyCacheKeys.Paged(filter.PageNumber, 
                filter.PageSize,
                filter.Code ?? string.Empty,
                filter.Name ?? string.Empty,
                filter.Country_Id?.ToString() ?? string.Empty
                );

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<PaginatedResponseDto<CurrencyDto>>(cached)!;

            var query = _uow.Currencies.GetQueryable()
                .Where(x => !x.Deleted);

            if (!string.IsNullOrEmpty(filter.Code))
                query = query.Where(x => x.Code.Contains(filter.Code));

            if (!string.IsNullOrEmpty(filter.Name))
                query = query.Where(x => x.Name.Contains(filter.Name));

            if (filter.Country_Id.HasValue)
                query = query.Where(x => x.Country_Id == filter.Country_Id);

            var totalRecords = await query.CountAsync();

            var data = await query
                .OrderByDescending(x => x.Create_Date)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(x => new CurrencyDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Symbol = x.Symbol,
                    Country_Id = x.Country_Id,
                    Country_Name = x.Country != null ? x.Country.Name : "",
                    Deleted = x.Deleted,
                    Published = x.Published,
                    Is_Active = x.Is_Active,
                    Create_Date = x.Create_Date,
                    Last_Update_Date = x.Last_Update_Date
                })
                .AsNoTracking()
                .ToListAsync();

            var result = new PaginatedResponseDto<CurrencyDto>
            {
                TotalRecords = totalRecords,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / filter.PageSize),
                Data = data
            };

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15) });

            return result;
        }

        

        public async Task<CurrencyDto> CreateAsync(CurrencyCreateDto dto, string userId)
        {
            var currency = new Currency
            {
                Id = Guid.NewGuid(),
                Code = dto.Code,
                Name = dto.Name,
                Symbol = dto.Symbol,
                Country_Id = dto.Country_Id,
                Create_Date = DateTime.UtcNow,
                Create_User = Guid.Parse(userId),
                Published = true,
                Deleted = false,
                Is_Active = true,
                RecordStatus = Blocks.RecordStatus.Active,
            };

            await _uow.Currencies.AddAsync(currency);
            await _uow.SaveAsync();

            // 🔥 CACHE INVALIDATION
            await _cache.RemoveAsync(CurrencyCacheKeys.All);

            return await GetByIdAsync(currency.Id) ?? new CurrencyDto();
        }

        public async Task<CurrencyDto> UpdateAsync(Guid id, CurrencyUpdateDto dto, string userId)
        {
            var currency = await _uow.Currencies.GetByIdAsync(id);
            if (currency == null) return new CurrencyDto();

            currency.Code = dto.Code;
            currency.Name = dto.Name;
            currency.Symbol = dto.Symbol;
            currency.Country_Id = dto.Country_Id;

            currency.Last_Update_Date = DateTime.UtcNow;
            currency.Last_Update_User = Guid.Parse(userId);

            _uow.Currencies.Update(currency);
            await _uow.SaveAsync();

            // 🔥 CACHE INVALIDATION
            await _cache.RemoveAsync(CurrencyCacheKeys.All);
            await _cache.RemoveAsync(CurrencyCacheKeys.ById(id));


            return await GetByIdAsync(id) ?? new CurrencyDto();
        }

        public async Task<CurrencyDto> DeleteAsync(Guid id, string userId)
        {
            var currency = await _uow.Currencies.GetByIdAsync(id);
            if (currency == null) return new CurrencyDto();

            currency.Deleted = true;
            currency.Published = false;
            currency.Is_Active = false;
            currency.RecordStatus = Blocks.RecordStatus.Inactive;
            currency.Last_Update_User = Guid.Parse(userId);
            currency.Last_Update_Date = DateTime.UtcNow;

            _uow.Currencies.Update(currency);
            await _uow.SaveAsync();

            // 🔥 CACHE INVALIDATION
            await _cache.RemoveAsync(CurrencyCacheKeys.All);
            await _cache.RemoveAsync(CurrencyCacheKeys.ById(id));

            return await GetByIdAsync(id) ?? new CurrencyDto();
        }
    }

}
