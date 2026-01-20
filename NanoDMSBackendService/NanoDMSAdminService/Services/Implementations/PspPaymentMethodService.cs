using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.Psp;
using NanoDMSAdminService.DTO.PspDocument;
using NanoDMSAdminService.DTO.PspPaymentMethod;
using NanoDMSAdminService.Filters;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Services.Interfaces;
using NanoDMSAdminService.UnitOfWorks;
using NanoDMSSharedLibrary.CacheKeys;
using System.Text.Json;

namespace NanoDMSAdminService.Services.Implementations
{
    public class PspPaymentMethodService : IPspPaymentMethodService
    {
        private readonly IUnitOfWork _uow;
        private readonly IDistributedCache _cache;

        public PspPaymentMethodService(IUnitOfWork uow, IDistributedCache cache)
        {
             _uow = uow;
            _cache = cache;
        }

        public async Task<IEnumerable<PspPaymentMethodDto>> GetAllAsync()
        {
            const string cacheKey = "psppaymentmethods:all";

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<IEnumerable<PspPaymentMethodDto>>(cached)!;

            var pspPaymentMethod = await _uow.PspPaymentMethods.GetAllByConditionAsync(b =>
                !b.Deleted && b.Is_Active
            );

            var result = pspPaymentMethod.Select(x => new PspPaymentMethodDto
            {
                Id = x.Id,
                Psp_Id = x.Psp_Id,
                Psp_Name = x.Psp.Name,
                Payment_Type = x.Payment_Type,
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

        public async Task<PspPaymentMethodDto?> GetByIdAsync(Guid id)
        {
            var cacheKey = $"psppaymentmethods:{id}";

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<PspPaymentMethodDto>(cached);

            var pspPaymentMethod = await _uow.PspPaymentMethods.GetByIdAsync(id);
            var dto = pspPaymentMethod == null ? null : MapToDto(pspPaymentMethod);

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dto), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15) });

            return dto;
        }

        public async Task<PaginatedResponseDto<PspPaymentMethodDto>> GetPagedAsync(PspPaymentMethodFilterModel filter)
        {
            var cacheKey = PspPaymentMethodCacheKeys.Paged(filter.PageNumber, filter.PageSize);

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<PaginatedResponseDto<PspPaymentMethodDto>>(cached)!;

            var query = _uow.PspPaymentMethods.GetQueryable();

            if (filter.Psp_Id.HasValue)
                query = query.Where(x => x.Psp_Id == filter.Psp_Id);

            if (filter.Payment_Type.HasValue)
                query = query.Where(x => x.Payment_Type == filter.Payment_Type);

            query = query.OrderByDescending(x => x.Create_Date);

            var totalRecords = await query.CountAsync();

            var pspDocument = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var result = new PaginatedResponseDto<PspPaymentMethodDto>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / filter.PageSize),
                Data = pspDocument.Select(MapToDto).ToList()
            };

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)});

            return result;
        }
        public async Task<PspPaymentMethodDto> CreateAsync(PspPaymentMethodCreateDto dto, string userId)
        {
            var pspPaymentMethod = new PspPaymentMethod
            {
                Id = Guid.NewGuid(),
                Psp_Id = dto.Psp_Id,
                Payment_Type = dto.Payment_Type,
                BusinessLocation_Id = dto.Business_Location_Id,
                Business_Id = dto.Business_Id,
                Is_Active = true,
                Deleted = false,
                Published = true,
                RecordStatus = Blocks.RecordStatus.Active,
                Create_Date = DateTime.UtcNow,
                Create_User = Guid.Parse(userId)
            };

            await _uow.PspPaymentMethods.AddAsync(pspPaymentMethod);
            await _uow.SaveAsync();

            // 🔥 CACHE INVALIDATION
            await _cache.RemoveAsync(PspPaymentMethodCacheKeys.All);

            return MapToDto(pspPaymentMethod);
        }

        public async Task<PspPaymentMethodDto> UpdateAsync(Guid id, PspPaymentMethodUpdateDto dto, string userId)
        {
            var entity = await _uow.PspPaymentMethods.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Psp Payment Method not found");

            entity.Psp_Id = dto.Psp_Id;
            entity.Payment_Type = dto.Payment_Type;
            entity.Last_Update_Date = DateTime.UtcNow;
            entity.Last_Update_User = Guid.Parse(userId);

            _uow.PspPaymentMethods.Update(entity);
            await _uow.SaveAsync();

            // 🔥 CACHE INVALIDATION
            await _cache.RemoveAsync(PspPaymentMethodCacheKeys.All);
            await _cache.RemoveAsync(PspPaymentMethodCacheKeys.ById(id));

            return MapToDto(entity);
        }
        public async Task<PspPaymentMethodDto> DeleteAsync(Guid id, string userId)
        {
            var pspPaymentMethod = await _uow.PspPaymentMethods.GetByIdAsync(id);
            if (pspPaymentMethod == null) return new PspPaymentMethodDto();

            pspPaymentMethod.Deleted = true;
            pspPaymentMethod.Published = false;
            pspPaymentMethod.Is_Active = false;
            pspPaymentMethod.RecordStatus = Blocks.RecordStatus.Inactive;
            pspPaymentMethod.Last_Update_Date = DateTime.UtcNow;
            pspPaymentMethod.Last_Update_User = Guid.Parse(userId);

            _uow.PspPaymentMethods.Update(pspPaymentMethod);
            await _uow.SaveAsync();

            // 🔥 CACHE INVALIDATION
            await _cache.RemoveAsync(PspPaymentMethodCacheKeys.All);
            await _cache.RemoveAsync(PspPaymentMethodCacheKeys.ById(id));

            return MapToDto(pspPaymentMethod);
        }
        private static PspPaymentMethodDto MapToDto(PspPaymentMethod x) => new()
        {
            Id = x.Id,
            Psp_Id = x.Psp_Id,
            Psp_Name = x.Psp.Name,
            Payment_Type = x.Payment_Type,
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
