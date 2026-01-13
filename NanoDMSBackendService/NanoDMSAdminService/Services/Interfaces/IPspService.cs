using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.Psp;
using NanoDMSAdminService.Filters;

namespace NanoDMSAdminService.Services.Interfaces
{
    public interface IPspService
    {
        Task<IEnumerable<PspDto>> GetAllAsync();
        Task<PaginatedResponseDto<PspDto>> GetPagedAsync(PspFilterModel filter);
        Task<PspDto?> GetByIdAsync(Guid id);
        Task<PspDto> CreateAsync(PspCreateDto dto, string userId);
        Task<PspDto> UpdateAsync(Guid id, PspUpdateDto dto, string userId);
        Task<PspDto> DeleteAsync(Guid id, string userId);
    }
}
