using NanoDMSSetupService.Common;

namespace NanoDMSSetupService.Models
{
    public class MaritalStatus : BaseEntity
    {   
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
