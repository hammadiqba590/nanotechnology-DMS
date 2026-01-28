using NanoDMSRightsService.DTO.Claims;

namespace NanoDMSRightsService.Services.Interfaces
{
    public interface IRightsCacheService
    {
        Task<UserClaimsDto> GetUserClaimsAsync(List<Guid> roleIds);
        Task InvalidateUserAsync(Guid userId);
    }

}
