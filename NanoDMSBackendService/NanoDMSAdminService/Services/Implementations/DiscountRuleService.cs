using Microsoft.EntityFrameworkCore;
using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.Currency;
using NanoDMSAdminService.DTO.DiscountRule;
using NanoDMSAdminService.Filters;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.UnitOfWorks;

namespace NanoDMSAdminService.Services.Implementations
{
    public class DiscountRuleService : IDiscountRuleService
    {
        private readonly IUnitOfWork _uow;

        public DiscountRuleService(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task<IEnumerable<DiscountRuleDto>> GetAllAsync()
        {
            var rules = await _uow.DiscountRules.GetAllByConditionAsync(b =>
                !b.Deleted && b.Is_Active
            );

            return rules.Select(b => new DiscountRuleDto
            {
                Id = b.Id,
                Campaign_Card_Bin_Id = b.Campaign_Card_Bin_Id,
                Discount_Type = b.Discount_Type,
                Discount_Value = b.Discount_Value,
                Min_Spend = b.Min_Spend,
                Max_Discount = b.Max_Discount,
                Payment_Type = b.Payment_Type,
                Budget_Limit_Type = b.Budget_Limit_Type,
                Budget_Limit_Value = b.Budget_Limit_Value,
                Applicable_Days = b.Applicable_Days,
                Transaction_Cap = b.Transaction_Cap,
                Priority = b.Priority,
                Start_Time = b.Start_Time,
                End_Time = b.End_Time,
                Deleted = b.Deleted,
                Published = b.Published,
                Create_Date = b.Create_Date,
                Create_User = b.Create_User,
                Last_Update_Date = b.Last_Update_Date,
                Last_Update_User = b.Last_Update_User,
                Business_Id = b.Business_Id,
                BusinessLocation_Id = b.BusinessLocation_Id,
                Is_Active = b.Is_Active,
                Status = b.Status,
            });
        }
        public async Task<PaginatedResponseDto<DiscountRuleDto>> GetPagedAsync(DiscountRuleFilterModel filter)
        {
            var query = _uow.DiscountRules.GetQueryable()
                .Where(x => !x.Deleted);

            if (filter.Campaign_Card_Bin_Id.HasValue)
                query = query.Where(x => x.Campaign_Card_Bin_Id == filter.Campaign_Card_Bin_Id);

            if (filter.Discount_Type.HasValue)
                query = query.Where(x => x.Discount_Type == filter.Discount_Type);

            if (filter.Payment_Type.HasValue)
                query = query.Where(x => x.Payment_Type == filter.Payment_Type);

            if (filter.Budget_Limit_Type.HasValue)
                query = query.Where(x => x.Budget_Limit_Type == filter.Budget_Limit_Type);

            var totalRecords = await query.CountAsync();

            var data = await query
                .OrderBy(x => x.Priority)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(x => new DiscountRuleDto
                {
                    Id = x.Id,
                    Campaign_Card_Bin_Id = x.Campaign_Card_Bin_Id,
                    Discount_Type = x.Discount_Type,
                    Discount_Value = x.Discount_Value,
                    Min_Spend = x.Min_Spend,
                    Max_Discount = x.Max_Discount,
                    Payment_Type = x.Payment_Type,
                    Budget_Limit_Type = x.Budget_Limit_Type,
                    Budget_Limit_Value = x.Budget_Limit_Value,
                    Applicable_Days = x.Applicable_Days,
                    Transaction_Cap = x.Transaction_Cap,
                    Priority = x.Priority,
                    Start_Time = x.Start_Time,
                    End_Time = x.End_Time,
                    Published = x.Published,
                    Deleted = x.Deleted,
                    Is_Active = x.Is_Active,
                    Create_Date = x.Create_Date,
                    Last_Update_Date = x.Last_Update_Date
                })
                .AsNoTracking()
                .ToListAsync();

            return new PaginatedResponseDto<DiscountRuleDto>
            {
                TotalRecords = totalRecords,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / filter.PageSize),
                Data = data
            };
        }

        public async Task<DiscountRuleDto?> GetByIdAsync(Guid id)
        {
            var rule = await _uow.DiscountRules.GetByIdAsync(id);
            if (rule == null) return null;

            return new DiscountRuleDto
            {
                Id = rule.Id,
                Campaign_Card_Bin_Id = rule.Campaign_Card_Bin_Id,
                Discount_Type = rule.Discount_Type,
                Discount_Value = rule.Discount_Value,
                Min_Spend = rule.Min_Spend,
                Max_Discount = rule.Max_Discount,
                Payment_Type = rule.Payment_Type,
                Budget_Limit_Type = rule.Budget_Limit_Type,
                Budget_Limit_Value = rule.Budget_Limit_Value,
                Applicable_Days = rule.Applicable_Days,
                Transaction_Cap = rule.Transaction_Cap,
                Priority = rule.Priority,
                Start_Time = rule.Start_Time,
                End_Time = rule.End_Time,
                Published = rule.Published,
                Deleted = rule.Deleted,
                Is_Active = rule.Is_Active,
                Create_Date = rule.Create_Date,
                Last_Update_Date = rule.Last_Update_Date
            };
        }

        public async Task<DiscountRuleDto> CreateAsync(DiscountRuleCreateDto dto, string userId)
        {
            var rule = new DiscountRule
            {
                Id = Guid.NewGuid(),
                Campaign_Card_Bin_Id = dto.Campaign_Card_Bin_Id,
                Discount_Type = dto.Discount_Type,
                Discount_Value = dto.Discount_Value,
                Min_Spend = dto.Min_Spend,
                Max_Discount = dto.Max_Discount,
                Payment_Type = dto.Payment_Type,
                Budget_Limit_Type = dto.Budget_Limit_Type,
                Budget_Limit_Value = dto.Budget_Limit_Value,
                Applicable_Days = dto.Applicable_Days,
                Transaction_Cap = dto.Transaction_Cap,
                Priority = dto.Priority,
                Start_Time = dto.Start_Time,
                End_Time = dto.End_Time,

                Create_Date = DateTime.UtcNow,
                Create_User = Guid.Parse(userId),
                Published = true,
                Deleted = false,
                Is_Active = true,
                Status = Blocks.RecordStatus.Active,
                Business_Id = dto.Business_Id,
                BusinessLocation_Id = dto.BusinessLocation_Id
            };

            await _uow.DiscountRules.AddAsync(rule);
            await _uow.SaveAsync();

            return await GetByIdAsync(rule.Id) ?? new DiscountRuleDto();
        }

        public async Task<DiscountRuleDto> UpdateAsync(Guid id, DiscountRuleUpdateDto dto, string userId)
        {
            var rule = await _uow.DiscountRules.GetByIdAsync(id);
            if (rule == null) return new DiscountRuleDto();

            rule.Discount_Type = dto.Discount_Type;
            rule.Discount_Value = dto.Discount_Value;
            rule.Min_Spend = dto.Min_Spend;
            rule.Max_Discount = dto.Max_Discount;
            rule.Payment_Type = dto.Payment_Type;
            rule.Budget_Limit_Type = dto.Budget_Limit_Type;
            rule.Budget_Limit_Value = dto.Budget_Limit_Value;
            rule.Applicable_Days = dto.Applicable_Days;
            rule.Transaction_Cap = dto.Transaction_Cap;
            rule.Priority = dto.Priority;
            rule.Start_Time = dto.Start_Time;
            rule.End_Time = dto.End_Time;

            rule.Last_Update_Date = DateTime.UtcNow;
            rule.Last_Update_User = Guid.Parse(userId);

            _uow.DiscountRules.Update(rule);
            await _uow.SaveAsync();

            return await GetByIdAsync(id) ?? new DiscountRuleDto();
        }

        public async Task<DiscountRuleDto> DeleteAsync(Guid id, string userId)
        {
            var rule = await _uow.DiscountRules.GetByIdAsync(id);
            if (rule == null) return new DiscountRuleDto();

            rule.Deleted = true;
            rule.Published = false;
            rule.Is_Active = false;
            rule.Status = Blocks.RecordStatus.Inactive;
            rule.Last_Update_User = Guid.Parse(userId);
            rule.Last_Update_Date = DateTime.UtcNow;

            _uow.DiscountRules.Update(rule);
            await _uow.SaveAsync();

            return await GetByIdAsync(id) ?? new DiscountRuleDto();
        }
    }

}
