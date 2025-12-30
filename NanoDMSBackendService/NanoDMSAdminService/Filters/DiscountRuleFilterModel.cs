using NanoDMSAdminService.Blocks;

namespace NanoDMSAdminService.Filters
{
    public class DiscountRuleFilterModel
    {
        public Guid? Campaign_Card_Bin_Id { get; set; }
        public DiscountTypeStatus? Discount_Type { get; set; }
        public PaymentTypeStatus? Payment_Type { get; set; }
        public BudgetLimitTypeStatus? Budget_Limit_Type { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

}
