using NanoDMSRightsService.Blocks;
using NanoDMSRightsService.Common;

namespace NanoDMSRightsService.DTO.Menu
{
    public class MenuDto : BaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Route { get; set; } = null!;
        public string? Icon { get; set; }
        public Guid? Parent_Id { get; set; }
        public Permissions Permissions { get; set; }
        public List<MenuDto> Children { get; set; } = new();
    }

}
