using NanoDMSRightsService.Common;

namespace NanoDMSRightsService.Models
{
    public class AuditLog : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid User_Id { get; set; }
        public string Action { get; set; } = null!;
        public string Entity { get; set; } = null!;
        public string? Old_Value { get; set; }
        public string? New_Value { get; set; }
        
    }

}
