using System.ComponentModel.DataAnnotations.Schema;
using NanoDMSSetupService.Common;

namespace NanoDMSSetupService.Models
{
    public class City : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid StateId { get; set; }

        [ForeignKey("StateId")]
        public State? State { get; set; } // Navigation property for State
        public string Name { get; set; } = string.Empty;
    }
}
