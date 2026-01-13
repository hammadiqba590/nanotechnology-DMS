using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.PspCategory;
using NanoDMSAdminService.Filters;
using NanoDMSAdminService.Models;

namespace NanoDMSAdminService.Services.Interfaces
{
    public interface IPspCategoryService
    {
        Task<IEnumerable<PspCategoryDto>> GetAllAsync();
        Task<PaginatedResponseDto<PspCategoryDto>> GetPagedAsync(PspCategoryFilterModel filter);
        Task<PspCategoryDto?> GetByIdAsync(Guid id);
        Task<PspCategoryDto> CreateAsync(PspCategoryCreateDto dto, string userId);
        Task<PspCategoryDto> UpdateAsync(Guid id, PspCategoryUpdateDto dto, string userId);
        Task<PspCategoryDto> DeleteAsync(Guid id, string userId);
    }
}
