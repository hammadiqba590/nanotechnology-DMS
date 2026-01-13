using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.PosTerminalConfiguration;
using NanoDMSAdminService.Filters;

namespace NanoDMSAdminService.Services.Interfaces
{
    public interface IPosTerminalConfigurationService
    {
        Task<IEnumerable<PosTerminalConfigurationDto>> GetAllAsync();
        Task<PaginatedResponseDto<PosTerminalConfigurationDto>> GetPagedAsync(PosTerminalConfigurationFilterModel filter);
        Task<PosTerminalConfigurationDto?> GetByIdAsync(Guid id);
        Task<PosTerminalConfigurationDto> CreateAsync(PosTerminalConfigurationCreateDto dto, string userId);
        Task<PosTerminalConfigurationDto> UpdateAsync(Guid id, PosTerminalConfigurationUpdateDto dto, string userId);
        Task<PosTerminalConfigurationDto> DeleteAsync(Guid id, string userId);
    }
}
