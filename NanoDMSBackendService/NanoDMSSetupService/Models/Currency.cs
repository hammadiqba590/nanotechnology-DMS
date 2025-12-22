using NanoDMSSetupService.Common;

namespace NanoDMSSetupService.Models
{
    public class Currency: BaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal ConversionRateToUSD { get; set; }
    }
}
