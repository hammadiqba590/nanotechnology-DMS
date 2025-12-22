using NanoDMSSetupService.Common;

namespace NanoDMSSetupService.Models
{
    public class Country : BaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Navigation property for related states
        public ICollection<State> States { get; set; } = new List<State>();
    }
}
