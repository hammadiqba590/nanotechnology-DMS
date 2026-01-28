using NanoDMSAdminService.Blocks;
using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.CampaignCardBin;

namespace NanoDMSAdminService.DTO.CampaignBank
{
    public class CampaignBankResponseDto : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid Campagin_Id { get; set; }
        public string Campaign_Name { get; set; } = "";
        public Guid Bank_Id { get; set; }
        public string Bank_Name { get; set; } = "";
        public decimal Budget { get; set; }
        public decimal Discount_Share { get; set; }  // -- % share of discount for bank
        public decimal Bank_Share { get; set; }
        public TaxOnMerchantStatus? Tax_On_Merchant_Share { get; set; }
        public BudgetLimitTypeStatus? Budget_Limit_Type { get; set; } //'hourly','daily','weekly','monthly','yearly'
        public int? Budget_Limit_Value { get; set; } //'Optional: max transactions or uses in the period',
        public DiscountModeStatus? Discount_Mode { get; set; }
        public RecordStatus? Status { get; set; }
        public List<CampaignCardBinResponseDto> CardBins { get; set; } = [];
    }
}
