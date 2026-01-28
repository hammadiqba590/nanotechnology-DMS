using NanoDMSRightsService.Blocks;

namespace NanoDMSRightsService.DTO.Menu
{
    public class MenuTreeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public List<Permissions> Permissions { get; set; } = new();
        public List<MenuTreeDto> Children { get; set; } = new();
    }

}
