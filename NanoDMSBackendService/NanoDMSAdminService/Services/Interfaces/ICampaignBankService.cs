using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.CampaignBank;
using NanoDMSAdminService.Filters;

namespace NanoDMSAdminService.Services.Interfaces
{
    public interface ICampaignBankService
    {
        Task<PaginatedResponseDto<CampaignBankDto>> GetPagedAsync(CampaignBankFilterModel filter);
        Task<IEnumerable<CampaignBankDto>> GetAllAsync();
        Task<CampaignBankDto?> GetByIdAsync(Guid id);
        Task<CampaignBankDto> CreateAsync(CampaignBankCreateDto dto, string userId);
        Task<CampaignBankDto> UpdateAsync(Guid id, CampaignBankUpdateDto dto, string userId);
        Task<CampaignBankDto> DeleteAsync(Guid id, string userId);
    }
}
