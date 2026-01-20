using NanoDMSAdminService.Blocks;
using NanoDMSAdminService.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NanoDMSAdminService.Models
{
    public class CampaignCardBin : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid Campagin_Id { get; set; }
        [ForeignKey(nameof(Campagin_Id))]
        public Campaign? Campaign { get; set; }
        [Required]
        public Guid Campagin_Bank_Id { get; set; }
        [ForeignKey(nameof(Campagin_Bank_Id))]
        public CampaignBank? Campaign_Bank { get; set; }
        [Required]
        public Guid Card_Bin_Id { get; set; }
        [ForeignKey(nameof(Card_Bin_Id))]
        public CardBin? Card_Bin { get; set; }
        public CampaginCardBinStatus? Status { get; set; }

        public ICollection<DiscountRule> Discount_Rules { get; set; } = new List<DiscountRule>();
        public ICollection<DiscountRuleHistory> Discount_Rule_Histories { get; set; } = new List<DiscountRuleHistory>();
    }
}
