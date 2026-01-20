using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.Bank;
using NanoDMSAdminService.Filters;

namespace NanoDMSAdminService.Services.Interfaces
{
    public interface IBankService
    {
        Task<IEnumerable<BankDto>> GetAllAsync();
        Task<BankDto?> GetByIdAsync(Guid id);
        Task<PaginatedResponseDto<BankDto>> GetPagedAsync(BankFilterModel filter);
        Task<BankDto> CreateAsync(BankCreateDto dto, string userId);
        Task<BankDto> UpdateAsync(Guid id, BankUpdateDto dto, string userId);
        Task<BankDto> DeleteAsync(Guid id, string userId);
    }
}
