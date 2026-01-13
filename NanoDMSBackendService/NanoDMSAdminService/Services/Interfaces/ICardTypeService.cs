using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.CardType;
using NanoDMSAdminService.Filters;

namespace NanoDMSAdminService.Services.Interfaces
{
    public interface ICardTypeService
    {
        Task<PaginatedResponseDto<CardTypeDto>> GetPagedAsync(CardTypeFilterModel filter);
        Task<IEnumerable<CardTypeDto>> GetAllAsync();
        Task<CardTypeDto?> GetByIdAsync(Guid id);
        Task<CardTypeDto> CreateAsync(CardTypeCreateDto dto, string userId);
        Task<CardTypeDto> UpdateAsync(Guid id, CardTypeUpdateDto dto, string userId);
        Task<CardTypeDto> DeleteAsync(Guid id, string userId);
    }
}
