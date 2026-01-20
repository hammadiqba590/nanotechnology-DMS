using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.Campagin;
using NanoDMSAdminService.Filters;
using NanoDMSAdminService.Models;

namespace NanoDMSAdminService.Services.Interfaces
{
    public interface ICampaignService
    {
        Task<PaginatedResponseDto<CampaignDto>> GetPagedAsync(CampaignFilterModel filter);
        Task<IEnumerable<CampaignDto>> GetAllAsync();
        Task<CampaignDto?> GetByIdAsync(Guid id);
        Task<CampaignDto> CreateAsync(CampaignCreateDto dto, string userId);
        Task<CampaignDto> UpdateAsync(Guid id, CampaignUpdateDto dto, string userId);
        Task<CampaignDto> DeleteAsync(Guid id, string userId);
        Task<CampaignFullResponseDto> CreateFullCampaignAsync(CampaignFullCreateDto dto,Guid userId);
        Task<List<Campaign>> GetActiveCampaignsByTerminalAsync(string serialNumber);
    }
}
