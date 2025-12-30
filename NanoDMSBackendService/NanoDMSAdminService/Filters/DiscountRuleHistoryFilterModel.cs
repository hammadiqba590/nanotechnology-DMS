using NanoDMSAdminService.Blocks;
using NanoDMSAdminService.Models;

namespace NanoDMSAdminService.Filters
{
    public class DiscountRuleHistoryFilterModel
    {
        public Guid? Discount_Rule_Id { get; set; }
        public Guid? Campaign_Card_Bin_Id { get; set; }
        public Guid? Currency_Id { get; set; }
        public DiscountTypeStatus? Discount_Type { get; set; }
        public PaymentTypeStatus? Payment_Type { get; set; }
        public ChangeTypeStatus? Change_Type { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

}
