using NanoDMSAdminService.Blocks;
using NanoDMSAdminService.Common;
using System.ComponentModel.DataAnnotations;

namespace NanoDMSAdminService.DTO.DiscountRule
{
    public class DiscountRuleCreateDto
    {
        public Guid Campaign_Card_Bin_Id { get; set; }

        public DiscountTypeStatus? Discount_Type { get; set; }
        public string? Discount_Mode { get; set; }
        public string? Pos_Mode { get; set; }
        public decimal Discount_Value { get; set; }
        public decimal? Min_Spend { get; set; }
        public decimal? Max_Discount { get; set; }

        public PaymentTypeStatus? Payment_Type { get; set; }
        public string? Budget_Limit_Type { get; set; }
        //public BudgetLimitTypeStatus? Budget_Limit_Type { get; set; }
        // public int? Budget_Limit_Value { get; set; }

        public string? Applicable_Days { get; set; }
        public int? Transaction_Cap { get; set; }
        public int Priority { get; set; }
        public bool Stackable { get; set; }
        public TimeSpan? Start_Time { get; set; }
        public TimeSpan? End_Time { get; set; }

        [Required]
        public Guid Business_Id { get; set; }
        [Required]
        public Guid BusinessLocation_Id { get; set; }
    }

}
