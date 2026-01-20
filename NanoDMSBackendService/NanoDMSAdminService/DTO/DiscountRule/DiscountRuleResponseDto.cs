using NanoDMSAdminService.Blocks;
using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.CampaignCardBin;
using NanoDMSAdminService.DTO.DiscountRuleHistory;

namespace NanoDMSAdminService.DTO.DiscountRule
{
    public class DiscountRuleResponseDto : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid Campaign_Card_Bin_Id { get; set; }
        public string Campaign_Name { get; set; } = "";
        public DiscountTypeStatus? Discount_Type { get; set; }
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
        public List<DiscountRuleHistoryDto> DiscountRuleHistories { get; set; } = [];
    }
}
