using NanoDMSAdminService.Blocks;
using NanoDMSAdminService.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NanoDMSAdminService.Models
{
    public class DiscountRuleHistory : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid Discount_Rule_Id { get; set; }
        [ForeignKey(nameof(Discount_Rule_Id))]
        public DiscountRule? DiscountRule { get; set; }
        public string? Discount_Mode { get; set; }
        public string? Pos_Mode { get; set; }
        public Guid Campaign_Card_Bin_Id { get; set; }
        [ForeignKey(nameof(Campaign_Card_Bin_Id))]
        public CampaignCardBin? CampaignCardBin { get; set; }
        public DiscountTypeStatus? Discount_Type { get; set; } 
        [Required]
        public decimal Discount_Value { get; set; } 
        public Guid Currency_Id { get; set; }
        [ForeignKey(nameof(Currency_Id))]
        public Currency? Currency { get; set; }
        public decimal? Min_Spend { get; set; }
        public decimal? Max_Discount { get; set; }
        public PaymentTypeStatus? Payment_Type { get; set; } // 'all','card','wallet','qr'

        public string? Applicable_Days { get; set; } // Json
        public int? Transaction_Cap { get; set; }
        public int Priority { get; set; }
        public bool Stackable { get; set; }
        
        public ChangeTypeStatus? Change_Type { get; set; }  // 'insert','update','delete'
    }

}
