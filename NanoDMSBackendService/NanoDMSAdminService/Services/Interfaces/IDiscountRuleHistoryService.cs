using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.DiscountRuleHistory;
using NanoDMSAdminService.Filters;

namespace NanoDMSAdminService.Services.Interfaces
{
    public interface IDiscountRuleHistoryService
    {
        Task<IEnumerable<DiscountRuleHistoryDto>> GetAllAsync();
        Task<PaginatedResponseDto<DiscountRuleHistoryDto>> GetPagedAsync(DiscountRuleHistoryFilterModel filter);
        Task<DiscountRuleHistoryDto?> GetByIdAsync(Guid id);
        Task<DiscountRuleHistoryDto> CreateAsync(DiscountRuleHistoryCreateDto dto, Guid userId);
        Task<DiscountRuleHistoryDto> UpdateAsync(Guid id, DiscountRuleHistoryUpdateDto dto, Guid userId);
        Task<DiscountRuleHistoryDto> DeleteAsync(Guid id, Guid userId);
    }
}
