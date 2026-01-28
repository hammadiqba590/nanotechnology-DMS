using NanoDMSAdminService.Blocks;
using NanoDMSAdminService.Common;

namespace NanoDMSAdminService.DTO.CampaignBank
{
    public class CampaignBankDto : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid Campagin_Id { get; set; }
        public Guid Bank_Id { get; set; }
        public string Bank_Name { get; set; } = "";
        public decimal Budget { get; set; }
        public decimal Discount_Share { get; set; }
        public decimal Bank_Share { get; set; }

        public TaxOnMerchantStatus? Tax_On_Merchant_Share { get; set; }
        public BudgetLimitTypeStatus? Budget_Limit_Type { get; set; }
        public int? Budget_Limit_Value { get; set; }

        public DiscountModeStatus? Discount_Mode { get; set; }
        public RecordStatus? Status { get; set; }

    }
}
