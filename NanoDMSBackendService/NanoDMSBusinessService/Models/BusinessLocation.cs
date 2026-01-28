using System.ComponentModel.DataAnnotations.Schema;
using NanoDMSBusinessService.Common;

namespace NanoDMSBusinessService.Models
{
    public class BusinessLocation: BaseEntity
    {
        public Guid Id { get; set; }
        public Guid Business_Id { get; set; }

        [ForeignKey("Business_Id")]
        public Business? Business { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public Guid Country { get; set; }
        public Guid State { get; set; }
        public Guid City { get; set; } 
        public string Postal_Code { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public int DiscountBeforeTax { get; set; }
        public decimal PosCharge { get; set; }
        public int DiscountBeforePosCharge { get; set; }
        public decimal ServiceCharges { get; set; }
        public int DiscountBeforeServiceCharge { get; set; }

        // Navigation property for BusinessLocationUser
        public ICollection<BusinessLocationUser> BusinessLocationUsers { get; set; } = new List<BusinessLocationUser>();

        public ICollection<BusinessConfig> BusinessConfigs { get; set; } = new List<BusinessConfig>();

        public ICollection<BusinessLocationPsp> Psps { get; set; }= new List<BusinessLocationPsp>();

        public ICollection<BusinessLocationBankSettlement> BankSettlements { get; set; } = new List<BusinessLocationBankSettlement>();

    }
}
