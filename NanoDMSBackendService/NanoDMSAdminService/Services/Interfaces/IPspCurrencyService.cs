using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.PspCurrency;
using NanoDMSAdminService.Filters;

namespace NanoDMSAdminService.Services.Interfaces
{
    public interface IPspCurrencyService
    {
        Task<IEnumerable<PspCurrencyDto>> GetAllAsync();
        Task<PaginatedResponseDto<PspCurrencyDto>> GetPagedAsync(PspCurrencyFilterModel filter);
        Task<PspCurrencyDto?> GetByIdAsync(Guid id);
        Task<PspCurrencyDto> CreateAsync(PspCurrencyCreateDto dto, string userId);
        Task<PspCurrencyDto> UpdateAsync(Guid id, PspCurrencyUpdateDto dto, string userId);
        Task<PspCurrencyDto> DeleteAsync(Guid id, string userId);
    }
}
