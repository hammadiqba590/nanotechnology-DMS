using NanoDMSRightsService.DTO.Claims;
using NanoDMSRightsService.DTO.Menu;
using NanoDMSRightsService.DTO.RoleMenu;
using NanoDMSRightsService.Models;

namespace NanoDMSRightsService.Services.Interfaces
{
    public interface IRightsService
    {
        Task AssignMenusAsync(AssignMenusToRoleDto dto, Guid performedByUserId);
        Task<UserClaimsDto> GetClaimsByRolesAsync(List<Guid> roleIds);
        Task<List<MenuDto>> GetMenusByRolesAsync(List<Guid> roleIds);
    }
}
