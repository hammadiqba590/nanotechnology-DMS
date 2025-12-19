using System.ComponentModel.DataAnnotations.Schema;
using NanoDMSBusinessService.Common;

namespace NanoDMSBusinessService.Models
{
    public class BusinessLocation: BaseEntity
    {
        public Guid Id { get; set; }
        public Guid BusinessId { get; set; }
        [ForeignKey("BusinessId")]
        public Business? Business { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public Guid Country { get; set; }
        public Guid State { get; set; }
        public Guid City { get; set; } 
        public string PostalCode { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;

        // Navigation property for BusinessLocationUser
        public ICollection<BusinessLocationUser> BusinessLocationUsers { get; set; } = new List<BusinessLocationUser>();

        public ICollection<BusinessConfig> BusinessConfigs { get; set; } = new List<BusinessConfig>();
    }
}
