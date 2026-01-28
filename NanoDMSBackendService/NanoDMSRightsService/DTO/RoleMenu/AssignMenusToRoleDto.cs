using NanoDMSRightsService.Blocks;
using NanoDMSRightsService.DTO.Menu;

namespace NanoDMSRightsService.DTO.RoleMenu
{
    public class AssignMenusToRoleDto
    {
        public Guid Role_Id { get; set; }
        public List<MenuPermissionDto> Menus { get; set; } = new();
    }


}
