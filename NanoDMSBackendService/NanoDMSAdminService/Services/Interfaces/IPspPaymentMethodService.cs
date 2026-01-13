using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.PspPaymentMethod;
using NanoDMSAdminService.Filters;

namespace NanoDMSAdminService.Services.Interfaces
{
    public interface IPspPaymentMethodService
    {
        Task<IEnumerable<PspPaymentMethodDto>> GetAllAsync();
        Task<PaginatedResponseDto<PspPaymentMethodDto>> GetPagedAsync(PspPaymentMethodFilterModel filter);
        Task<PspPaymentMethodDto?> GetByIdAsync(Guid id);
        Task<PspPaymentMethodDto> CreateAsync(PspPaymentMethodCreateDto dto, string userId);
        Task<PspPaymentMethodDto> UpdateAsync(Guid id, PspPaymentMethodUpdateDto dto, string userId);
        Task<PspPaymentMethodDto> DeleteAsync(Guid id, string userId);
    }
}
