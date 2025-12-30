using NanoDMSAdminService.Blocks;
using NanoDMSAdminService.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NanoDMSAdminService.Models
{
    public class DiscountRule : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid Campaign_Card_Bin_Id { get; set; }

        [ForeignKey(nameof(Campaign_Card_Bin_Id))]
        public CampaignCardBin? CampaignCardBin { get; set; }
        public DiscountTypeStatus? Discount_Type { get; set; } // 'percentage','flat'
        public decimal Discount_Value { get; set; }
        public decimal? Min_Spend { get; set; }
        public decimal? Max_Discount { get; set; }
        public PaymentTypeStatus? Payment_Type { get; set; } // ('all','card','wallet','qr'
        public BudgetLimitTypeStatus? Budget_Limit_Type { get; set; } // 'hourly','daily','weekly','monthly','yearly'
        public int? Budget_Limit_Value { get; set; } // 'Optional: max transactions or uses in the period',
        public string? Applicable_Days { get; set; } // Json
        public int? Transaction_Cap { get; set; }
        public int Priority { get; set; }
        public TimeSpan? Start_Time { get; set; }
        public TimeSpan? End_Time { get; set; }

        public ICollection<DiscountRuleHistory> Discount_Rule_Histories { get; set; } = new List<DiscountRuleHistory>();
    }

}
