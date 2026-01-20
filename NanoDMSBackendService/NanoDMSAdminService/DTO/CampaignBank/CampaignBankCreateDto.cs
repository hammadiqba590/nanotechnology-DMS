using NanoDMSAdminService.Blocks;
using NanoDMSAdminService.DTO.CampaignCardBin;
using System.ComponentModel.DataAnnotations;

namespace NanoDMSAdminService.DTO.CampaignBank
{
    public class CampaignBankCreateDto
    {
        [Required]
        public Guid Campagin_Id { get; set; }

        [Required]
        public Guid Bank_Id { get; set; }

        public decimal Budget { get; set; }
        public decimal Discount_Share { get; set; }

        public TaxOnMerchantStatus? Tax_On_Merchant_Share { get; set; }
        public BudgetLimitTypeStatus? Budget_Limit_Type { get; set; }
        public int? Budget_Limit_Value { get; set; }

        public DiscountModeStatus? Discount_Mode { get; set; }
        public DateTime Start_Date { get; set; }
        public DateTime End_Date { get; set; }

        [Required]
        public Guid Business_Id { get; set; }

        [Required]
        public Guid Business_Location_Id { get; set; }
        public List<CampaignCardBinCreateDto> CardBins { get; set; } = [];
    }

}
