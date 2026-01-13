using NanoDMSAdminService.Blocks;
using NanoDMSAdminService.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NanoDMSAdminService.Models
{
    public class CampaignBank: BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid Campagin_Id { get; set; }
        [ForeignKey(nameof(Campagin_Id))]
        public Campaign? Campaign { get; set; }
        [Required]
        public Guid Bank_Id { get; set; }
        [ForeignKey(nameof(Bank_Id))]
        public Bank? Bank{ get; set; }
        public decimal Budget { get; set; }
        public decimal Discount_Share { get; set; }  // -- % share of discount for bank
        public TaxOnMerchantStatus? Tax_On_Merchant_Share { get; set; }
        public BudgetLimitTypeStatus? Budget_Limit_Type { get; set; } //'hourly','daily','weekly','monthly','yearly'
        public int? Budget_Limit_Value { get; set; } //'Optional: max transactions or uses in the period',
        public DiscountModeStatus? Discount_Mode { get; set; }
        public RecordStatus? Status { get; set; }
        public ICollection<CampaignCardBin> Campaign_Card_Bins { get; set; } = new List<CampaignCardBin>();
    }
}
