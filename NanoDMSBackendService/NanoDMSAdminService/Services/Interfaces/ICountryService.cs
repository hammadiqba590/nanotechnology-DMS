using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.Country;
using NanoDMSAdminService.Filters;

namespace NanoDMSAdminService.Services.Implementations
{
    public interface ICountryService
    {
        Task<PaginatedResponseDto<CountryDto>> GetPagedAsync(CountryFilterModel filter);
        Task<IEnumerable<CountryDto>> GetAllAsync();
        Task<CountryDto?> GetByIdAsync(Guid id);
        Task<CountryDto> CreateAsync(CountryCreateDto dto, string userId);
        Task<CountryDto> UpdateAsync(Guid id, CountryUpdateDto dto, string userId);
        Task<CountryDto> DeleteAsync(Guid id, string userId);
    }

}
