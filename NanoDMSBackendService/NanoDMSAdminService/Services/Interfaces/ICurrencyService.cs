using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.Country;
using NanoDMSAdminService.DTO.Currency;
using NanoDMSAdminService.Filters;

namespace NanoDMSAdminService.Services.Implementations
{
    public interface ICurrencyService
    {
        Task<PaginatedResponseDto<CurrencyDto>> GetPagedAsync(CurrencyFilterModel filter);
        Task<IEnumerable<CurrencyDto>> GetAllAsync();
        Task<CurrencyDto?> GetByIdAsync(Guid id);
        Task<CurrencyDto> CreateAsync(CurrencyCreateDto dto, string userId);
        Task<CurrencyDto> UpdateAsync(Guid id, CurrencyUpdateDto dto, string userId);
        Task<CurrencyDto> DeleteAsync(Guid id, string userId);
    }

}
