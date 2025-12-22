using NanoDMSSetupService.Common;

namespace NanoDMSSetupService.Models
{
    public class StockAccountingMethod: BaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
