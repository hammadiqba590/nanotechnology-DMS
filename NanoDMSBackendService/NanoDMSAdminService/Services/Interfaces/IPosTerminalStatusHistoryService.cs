using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.PosTerminalStatusHistory;
using NanoDMSAdminService.Filters;

namespace NanoDMSAdminService.Services.Interfaces
{
    public interface IPosTerminalStatusHistoryService
    {
        Task<IEnumerable<PosTerminalStatusHistoryDto>> GetAllAsync();
        Task<PaginatedResponseDto<PosTerminalStatusHistoryDto>> GetPagedAsync(PosTerminalStatusHistoryFilterModel filter);
        Task<PosTerminalStatusHistoryDto?> GetByIdAsync(Guid id);
        Task<PosTerminalStatusHistoryDto> CreateAsync(PosTerminalStatusHistoryCreateDto dto, string userId);
        Task<PosTerminalStatusHistoryDto> UpdateAsync(Guid id, PosTerminalStatusHistoryUpdateDto dto, string userId);
        Task<PosTerminalStatusHistoryDto> DeleteAsync(Guid id, string userId);
    }
}
