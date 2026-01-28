using NanoDMSRightsService.Common;

namespace NanoDMSRightsService.Models
{
    public class Menu : BaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!; // unique key
        public string Route { get; set; } = null!;
        public string Icon { get; set; } = null!;
        public int Order { get; set; }
        public Guid? Parent_Id { get; set; }
        public Menu? Parent { get; set; }

        public ICollection<Menu> Children { get; set; } = new List<Menu>();
        public ICollection<RoleMenuPermission> Role_Menu_Permissions { get; set; } = new List<RoleMenuPermission>();
    }

}
