using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using NanoDMSAdminService.Blocks;
using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.Currency;
using NanoDMSAdminService.DTO.DiscountRule;
using NanoDMSAdminService.DTO.DiscountRuleHistory;
using NanoDMSAdminService.Filters;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Services.Interfaces;
using NanoDMSAdminService.UnitOfWorks;
using NanoDMSSharedLibrary.CacheKeys;
using System.Text.Json;

namespace NanoDMSAdminService.Services.Implementations
{
    public class DiscountRuleHistoryService : IDiscountRuleHistoryService
    {
        private readonly IUnitOfWork _uow;
        private readonly IDistributedCache _cache;

        public DiscountRuleHistoryService(IUnitOfWork uow, IDistributedCache cache)
        {
            _uow = uow;
            _cache = cache;
        }

        //Get All
        public async Task<IEnumerable<DiscountRuleHistoryDto>> GetAllAsync()
        {
            const string cacheKey = "discountrulehistories:all";

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<IEnumerable<DiscountRuleHistoryDto>>(cached)!;

            var rules = await _uow.DiscountRuleHistories.GetAllByConditionAsync(b => !b.Deleted && b.Is_Active);

            var result = rules.Select(b => new DiscountRuleHistoryDto
            {
                Id = b.Id,
                Discount_Rule_Id = b.Discount_Rule_Id,
                Campaign_Card_Bin_Id = b.Campaign_Card_Bin_Id,
                Currency_Id = b.Currency_Id,
                Discount_Type = b.Discount_Type,
                Discount_Value = b.Discount_Value,
                Min_Spend = b.Min_Spend,
                Max_Discount = b.Max_Discount,
                Payment_Type = b.Payment_Type,
                Change_Type = b.Change_Type,
                Applicable_Days = b.Applicable_Days,
                Transaction_Cap = b.Transaction_Cap,
                Priority = b.Priority,
                Stackable = b.Stackable,
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
        public async Task<DiscountRuleHistoryDto?> GetByIdAsync(Guid id)
        {
            var cacheKey = $"discountrulehistories:{id}";

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<DiscountRuleHistoryDto>(cached);

            var x = await _uow.DiscountRuleHistories.GetByIdAsync(id);
            if (x == null) return null;

            var dto = MapToDto(x);

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dto), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15) });

            return dto;
        }
        // Get paginated histories
        public async Task<PaginatedResponseDto<DiscountRuleHistoryDto>> GetPagedAsync(DiscountRuleHistoryFilterModel filter)
        {
            var cacheKey = DiscountRuleHistoryCacheKeys.Paged(filter.PageNumber, filter.PageSize);

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<PaginatedResponseDto<DiscountRuleHistoryDto>>(cached)!;

            var query = _uow.DiscountRuleHistories.GetQueryable().Where(x => !x.Deleted);

            if (filter.Discount_Rule_Id.HasValue)
                query = query.Where(x => x.Discount_Rule_Id == filter.Discount_Rule_Id.Value);

            if (filter.Currency_Id.HasValue)
                query = query.Where(x => x.Currency_Id == filter.Currency_Id.Value);

            if (filter.Campaign_Card_Bin_Id.HasValue)
                query = query.Where(x => x.Campaign_Card_Bin_Id == filter.Campaign_Card_Bin_Id.Value);

            if (filter.Discount_Type.HasValue)
                query = query.Where(x => x.Discount_Type == filter.Discount_Type);

            if (filter.Payment_Type.HasValue)
                query = query.Where(x => x.Payment_Type == filter.Payment_Type);

            if (filter.Change_Type.HasValue)
                query = query.Where(x => x.Change_Type == filter.Change_Type);

            query = query.OrderByDescending(x => x.Create_Date);

            var totalRecords = await query.CountAsync();

            var data = await query
                .OrderBy(x => x.Priority)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var dtoData = data.Select(x => new DiscountRuleHistoryDto
            {
                Id = x.Id,
                Discount_Rule_Id = x.Discount_Rule_Id,
                Campaign_Card_Bin_Id = x.Campaign_Card_Bin_Id,
                Discount_Type = x.Discount_Type,
                Discount_Value = x.Discount_Value,
                Currency_Id = x.Currency_Id,
                Min_Spend = x.Min_Spend,
                Max_Discount = x.Max_Discount,
                Payment_Type = x.Payment_Type,
                Applicable_Days = x.Applicable_Days,
                Transaction_Cap = x.Transaction_Cap,
                Priority = x.Priority,
                Stackable = x.Stackable,
                Change_Type = x.Change_Type,
                Is_Active = x.Is_Active,
                Deleted = x.Deleted,
                Published = x.Published,
                Create_Date = x.Create_Date,
                Create_User = x.Create_User,
                Last_Update_Date = x.Last_Update_Date,
                Last_Update_User = x.Last_Update_User,
                RecordStatus = x.RecordStatus
            }).ToList();

            var result = new PaginatedResponseDto<DiscountRuleHistoryDto>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / filter.PageSize),
                Data = dtoData
            };

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)});

            return result;
        }

       

        public async Task<DiscountRuleHistoryDto> CreateAsync(DiscountRuleHistoryCreateDto dto, Guid userId)
        {
            var entity = new DiscountRuleHistory
            {
                Id = Guid.NewGuid(),
                Discount_Rule_Id = dto.Discount_Rule_Id,
                Campaign_Card_Bin_Id = dto.Campaign_Card_Bin_Id,
                Discount_Type = dto.Discount_Type,
                Discount_Value = dto.Discount_Value,
                Currency_Id = dto.Currency_Id,
                Min_Spend = dto.Min_Spend,
                Max_Discount = dto.Max_Discount,
                Payment_Type = dto.Payment_Type,
                Applicable_Days = dto.Applicable_Days,
                Transaction_Cap = dto.Transaction_Cap,
                Priority = dto.Priority,
                Stackable = dto.Stackable,
                Change_Type = dto.Change_Type,
                Deleted = false,
                Published = true,
                Is_Active = true,
                RecordStatus = RecordStatus.Active,
                Create_Date = DateTime.UtcNow,
                Create_User = userId
            };

            await _uow.DiscountRuleHistories.AddAsync(entity);
            await _uow.SaveAsync();

            // 🔥 CACHE INVALIDATION
            await _cache.RemoveAsync(DiscountRuleHistoryCacheKeys.All);


            return MapToDto(entity);
        }

        public async Task<DiscountRuleHistoryDto> UpdateAsync(Guid id, DiscountRuleHistoryUpdateDto dto, Guid userId)
        {
            var entity = await _uow.DiscountRuleHistories.GetByIdAsync(id);
            if (entity == null) return new DiscountRuleHistoryDto();

            entity.Discount_Rule_Id = dto.Discount_Rule_Id;
            entity.Campaign_Card_Bin_Id = dto.Campaign_Card_Bin_Id;
            entity.Discount_Type = dto.Discount_Type;
            entity.Discount_Value = dto.Discount_Value;
            entity.Currency_Id = dto.Currency_Id;
            entity.Min_Spend = dto.Min_Spend;
            entity.Max_Discount = dto.Max_Discount;
            entity.Payment_Type = dto.Payment_Type;
            entity.Applicable_Days = dto.Applicable_Days;
            entity.Transaction_Cap = dto.Transaction_Cap;
            entity.Priority = dto.Priority;
            entity.Stackable = dto.Stackable;
            entity.Change_Type = dto.Change_Type;

            entity.Last_Update_Date = DateTime.UtcNow;
            entity.Last_Update_User = userId;

            _uow.DiscountRuleHistories.Update(entity);
            await _uow.SaveAsync();

            // 🔥 CACHE INVALIDATION
            await _cache.RemoveAsync(DiscountRuleHistoryCacheKeys.All);
            await _cache.RemoveAsync(DiscountRuleHistoryCacheKeys.ById(id));

            return MapToDto(entity);
        }

        public async Task<DiscountRuleHistoryDto> DeleteAsync(Guid id, Guid userId)
        {
            var entity = await _uow.DiscountRuleHistories.GetByIdAsync(id);
            if (entity == null) return new DiscountRuleHistoryDto();

            entity.Deleted = true;
            entity.Published = false;
            entity.Is_Active = false;
            entity.RecordStatus = RecordStatus.Inactive;
            entity.Last_Update_Date = DateTime.UtcNow;
            entity.Last_Update_User = userId;

            _uow.DiscountRuleHistories.Update(entity);
            await _uow.SaveAsync();

            // 🔥 CACHE INVALIDATION
            await _cache.RemoveAsync(DiscountRuleHistoryCacheKeys.All);
            await _cache.RemoveAsync(DiscountRuleHistoryCacheKeys.ById(id));

            return MapToDto(entity);
        }

        private DiscountRuleHistoryDto MapToDto(DiscountRuleHistory x) => new()
        {
            Id = x.Id,
            Discount_Rule_Id = x.Discount_Rule_Id,
            Campaign_Card_Bin_Id = x.Campaign_Card_Bin_Id,
            Discount_Type = x.Discount_Type,
            Discount_Value = x.Discount_Value,
            Currency_Id = x.Currency_Id,
            Min_Spend = x.Min_Spend,
            Max_Discount = x.Max_Discount,
            Payment_Type = x.Payment_Type,
            Applicable_Days = x.Applicable_Days,
            Transaction_Cap = x.Transaction_Cap,
            Priority = x.Priority,
            Stackable = x.Stackable,
            Change_Type = x.Change_Type,
            Business_Id = x.Business_Id,
            BusinessLocation_Id = x.BusinessLocation_Id,
            Is_Active = x.Is_Active,
            Deleted = x.Deleted,
            Published = x.Published,
            Create_Date = x.Create_Date,
            Create_User = x.Create_User,
            Last_Update_Date = x.Last_Update_Date,
            Last_Update_User = x.Last_Update_User,
            RecordStatus = x.RecordStatus
        };
    }

}
