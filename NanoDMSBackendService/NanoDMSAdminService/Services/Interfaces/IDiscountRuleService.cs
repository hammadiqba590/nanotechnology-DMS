using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.Currency;
using NanoDMSAdminService.DTO.DiscountRule;
using NanoDMSAdminService.Filters;

namespace NanoDMSAdminService.Services.Interfaces
{
    public interface IDiscountRuleService
    {
        Task<IEnumerable<DiscountRuleDto>> GetAllAsync();
        Task<PaginatedResponseDto<DiscountRuleDto>> GetPagedAsync(DiscountRuleFilterModel filter);
        Task<DiscountRuleDto?> GetByIdAsync(Guid id);
        Task<DiscountRuleDto> CreateAsync(DiscountRuleCreateDto dto, string userId);
        Task<DiscountRuleDto> UpdateAsync(Guid id, DiscountRuleUpdateDto dto, string userId);
        Task<DiscountRuleDto> DeleteAsync(Guid id, string userId);
    }

}
