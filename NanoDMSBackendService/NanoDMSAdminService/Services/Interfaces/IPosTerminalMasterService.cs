using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.PosTerminalMaster;
using NanoDMSAdminService.Filters;

namespace NanoDMSAdminService.Services.Interfaces
{
    public interface IPosTerminalMasterService
    {
        Task<IEnumerable<PosTerminalMasterDto>> GetAllAsync();
        Task<PaginatedResponseDto<PosTerminalMasterDto>> GetPagedAsync(PosTerminalMasterFilterModel filter);
        Task<PosTerminalMasterDto?> GetByIdAsync(Guid id);
        Task<PosTerminalMasterDto> CreateAsync(PosTerminalMasterCreateDto dto, string userId);
        Task<PosTerminalMasterDto> UpdateAsync(Guid id, PosTerminalMasterUpdateDto dto, string userId);
        Task<PosTerminalMasterDto> DeleteAsync(Guid id, string userId);
    }
}
