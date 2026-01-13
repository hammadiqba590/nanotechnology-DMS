using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.CardLevel;
using NanoDMSAdminService.Filters;

namespace NanoDMSAdminService.Services.Interfaces
{
    public interface ICardLevelService
    {
        Task<PaginatedResponseDto<CardLevelDto>> GetPagedAsync(CardLevelFilterModel filter);
        Task<IEnumerable<CardLevelDto>> GetAllAsync();
        Task<CardLevelDto?> GetByIdAsync(Guid id);
        Task<CardLevelDto> CreateAsync(CardLevelCreateDto dto, string userId);
        Task<CardLevelDto> UpdateAsync(Guid id, CardLevelUpdateDto dto, string userId);
        Task<CardLevelDto> DeleteAsync(Guid id, string userId);
    }
}
