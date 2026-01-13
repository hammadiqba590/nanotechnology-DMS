using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.PosTerminalAssignment;
using NanoDMSAdminService.Filters;

namespace NanoDMSAdminService.Services.Interfaces
{
    public interface IPosTerminalAssignmentService
    {
        Task<IEnumerable<PosTerminalAssignmentDto>> GetAllAsync();
        Task<PaginatedResponseDto<PosTerminalAssignmentDto>> GetPagedAsync(PosTerminalAssignmentFilterModel filter);
        Task<PosTerminalAssignmentDto?> GetByIdAsync(Guid id);
        Task<PosTerminalAssignmentDto> CreateAsync(PosTerminalAssignmentCreateDto dto, string userId);
        Task<PosTerminalAssignmentDto> UpdateAsync(Guid id, PosTerminalAssignmentUpdateDto dto, string userId);
        Task<PosTerminalAssignmentDto> DeleteAsync(Guid id, string userId);
    }
}
