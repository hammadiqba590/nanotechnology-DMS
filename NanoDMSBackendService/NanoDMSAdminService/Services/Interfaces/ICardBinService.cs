using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.CardBin;
using NanoDMSAdminService.Filters;

namespace NanoDMSAdminService.Services.Interfaces
{
    public interface ICardBinService
    {
        Task<PaginatedResponseDto<CardBinDto>> GetPagedAsync(CardBinFilterModel filter);
        Task<IEnumerable<CardBinDto>> GetAllAsync();
        Task<CardBinDto?> GetByIdAsync(Guid id);
        Task<CardBinDto> CreateAsync(CardBinCreateDto dto, string userId);
        Task<CardBinDto> UpdateAsync(Guid id, CardBinUpdateDto dto, string userId);
        Task<CardBinDto> DeleteAsync(Guid id, string userId);
    }
}
