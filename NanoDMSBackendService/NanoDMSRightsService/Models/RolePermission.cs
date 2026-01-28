using NanoDMSRightsService.Common;

namespace NanoDMSRightsService.Models
{
    public class RolePermission : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }
        public Permission Permission { get; set; } = null!;
    }

}
