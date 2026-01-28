using NanoDMSAdminService.Blocks;
using NanoDMSAdminService.Common;

namespace NanoDMSAdminService.DTO.DiscountRuleHistory
{
    public class DiscountRuleHistoryDto : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid Discount_Rule_Id { get; set; }
        public Guid Campaign_Card_Bin_Id { get; set; }
        public DiscountTypeStatus? Discount_Type { get; set; }
        public string? Discount_Mode { get; set; }
        public string? Pos_Mode { get; set; }
        public decimal Discount_Value { get; set; }
        public Guid Currency_Id { get; set; }
        public decimal? Min_Spend { get; set; }
        public decimal? Max_Discount { get; set; }
        public PaymentTypeStatus? Payment_Type { get; set; }
        public string? Applicable_Days { get; set; }
        public int? Transaction_Cap { get; set; }
        public int Priority { get; set; }
        public bool Stackable { get; set; }
        public ChangeTypeStatus? Change_Type { get; set; }
    }

}
