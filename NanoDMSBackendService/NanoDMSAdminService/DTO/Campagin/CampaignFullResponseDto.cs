using NanoDMSAdminService.Blocks;
using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.CampaignBank;

namespace NanoDMSAdminService.DTO.Campagin
{
    public class CampaignFullResponseDto : BaseEntity
    {
        public Guid Id { get; set; }
        public string Campaign_Name { get; set; } = "";
        public string? Description { get; set; }
        public Guid Currency_Id { get; set; }
        public string Currency_Name { get; set; } = "";
        public Guid Psp_Id { get; set; }
        public string Psp_Name { get; set; } = "";
        public decimal? Tax_Amount { get; set; }
        public string? Fbr { get; set; }
        public CampaginStatus? Status { get; set; } //active, inactive,expired 
        public BudgetLimitTypeStatus? Budget_Limit_Type { get; set; } // 'hourly','daily','weekly','monthly','yearly'
        public int? Budget_Limit_Value { get; set; } //'Optional: max transactions or uses in the period',
        public int Priority { get; set; }
        public List<CampaignBankResponseDto> Banks { get; set; } = [];
    }
}
