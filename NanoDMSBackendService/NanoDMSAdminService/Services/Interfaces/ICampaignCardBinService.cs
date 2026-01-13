using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.CampaignCardBin;
using NanoDMSAdminService.Filters;

namespace NanoDMSAdminService.Services.Interfaces
{
    public interface ICampaignCardBinService
    {
        Task<PaginatedResponseDto<CampaignCardBinDto>> GetPagedAsync(CampaignCardBinFilterModel filter);
        Task<IEnumerable<CampaignCardBinDto>> GetAllAsync();
        Task<CampaignCardBinDto?> GetByIdAsync(Guid id);
        Task<CampaignCardBinDto> CreateAsync(CampaignCardBinCreateDto dto, string userId);
        Task<CampaignCardBinDto> UpdateAsync(Guid id, CampaignCardBinUpdateDto dto, string userId);
        Task<CampaignCardBinDto> DeleteAsync(Guid id, string userId);
    }
}
