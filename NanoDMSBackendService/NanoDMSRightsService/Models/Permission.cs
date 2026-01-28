using NanoDMSRightsService.Common;

namespace NanoDMSRightsService.Models
{
    public class Permission : BaseEntity
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = null!; // BANK.CREATE
        public string Name { get; set; } = null!; // Create Bank
        public string Module { get; set; } = null!; // Bank
    }
}
