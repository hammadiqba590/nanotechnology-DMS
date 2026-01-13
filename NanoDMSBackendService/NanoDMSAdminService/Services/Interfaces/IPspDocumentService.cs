using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.PspDocument;
using NanoDMSAdminService.Filters;

namespace NanoDMSAdminService.Services.Interfaces
{
    public interface IPspDocumentService
    {
        Task<IEnumerable<PspDocumentDto>> GetAllAsync();
        Task<PaginatedResponseDto<PspDocumentDto>> GetPagedAsync(PspDocumentFilterModel filter);
        Task<PspDocumentDto?> GetByIdAsync(Guid id);
        Task<PspDocumentDto> CreateAsync(PspDocumentCreateDto dto, string userId);
        Task<PspDocumentDto> UpdateAsync(Guid id, PspDocumentUpdateDto dto, string userId);
        Task<PspDocumentDto> DeleteAsync(Guid id, string userId);
    }
}
