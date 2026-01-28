using NanoDMSRightsService.Blocks;
using NanoDMSRightsService.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace NanoDMSRightsService.Models
{
    public class RoleMenuPermission : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid Role_Id { get; set; }
        [ForeignKey(nameof(Menu))] // <- explicitly tell EF this is the FK
        public Guid Menu_Id { get; set; }
        public Menu Menu { get; set; } = null!;

        // Must be string
        public Permissions Permissions { get; set; }
    }


}
