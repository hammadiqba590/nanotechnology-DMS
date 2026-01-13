using NanoDMSAdminService.Blocks;

namespace NanoDMSAdminService.Filters
{
    public class CampaignBankFilterModel
    {
        public string? Campaign_Name { get; set; }
        public string? Bank_Name { get; set; }
        public TaxOnMerchantStatus? Tax_On_Merchant_Share { get; set; }
        public BudgetLimitTypeStatus? Budget_Limit_Type { get; set; }
        public DiscountModeStatus? Discount_Mode { get; set; }
        public RecordStatus? Status { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
