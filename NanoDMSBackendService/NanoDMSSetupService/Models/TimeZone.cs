using NanoDMSSetupService.Common;

namespace NanoDMSSetupService.Models
{
    public class TimeZone : BaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string GMTSetting { get; set; } = string.Empty;
    }
}
