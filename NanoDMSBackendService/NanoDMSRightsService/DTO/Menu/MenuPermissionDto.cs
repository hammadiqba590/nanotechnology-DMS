using NanoDMSRightsService.Blocks;

namespace NanoDMSRightsService.DTO.Menu
{
    public class MenuPermissionDto
    {
        public Guid Menu_Id { get; set; }
        public Permissions Permissions { get; set; }
    }
}
