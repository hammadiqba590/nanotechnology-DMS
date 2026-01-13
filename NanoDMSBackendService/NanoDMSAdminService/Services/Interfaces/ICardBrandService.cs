using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.CardBrand;
using NanoDMSAdminService.Filters;

namespace NanoDMSAdminService.Services.Interfaces
{
    public interface ICardBrandService
    {
        Task<PaginatedResponseDto<CardBrandDto>> GetPagedAsync(CardBrandFilterModel filter);
        Task<IEnumerable<CardBrandDto>> GetAllAsync();
        Task<CardBrandDto?> GetByIdAsync(Guid id);
        Task<CardBrandDto> CreateAsync(CardBrandCreateDto dto, string userId);
        Task<CardBrandDto> UpdateAsync(Guid id, CardBrandUpdateDto dto, string userId);
        Task<CardBrandDto> DeleteAsync(Guid id, string userId);
    }
}
