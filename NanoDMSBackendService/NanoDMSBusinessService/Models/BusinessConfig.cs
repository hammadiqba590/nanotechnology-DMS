using System.ComponentModel.DataAnnotations.Schema;
using NanoDMSBusinessService.Common;

namespace NanoDMSBusinessService.Models
{
    public class BusinessConfig: BaseEntity
    {
        public Guid Id { get; set; }
        public string NameKey { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ConfigValue { get; set; } = string.Empty;
        public string ConfigType { get; set; } = string.Empty;
        public Guid BusinessId { get; set; }

        [ForeignKey("BusinessId")]
        public Business? Business { get; set; }

        public Guid BusinessLocationId { get; set; }

        [ForeignKey("BusinessLocationId")]
        public BusinessLocation? BusinessLocation { get; set; }

    }
}
