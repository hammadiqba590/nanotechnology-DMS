using NanoDMSAdminService.Blocks;
using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.CampaignBank;

namespace NanoDMSAdminService.DTO.Campagin
{
    public class CampaignDto : BaseEntity
    {
        public Guid Id { get; set; }
        public string Campaign_Name { get; set; } = "";
        public string? Description { get; set; }
        public Guid Currency_Id { get; set; }
        public string Currency_Name { get; set; } = "";
        public decimal? Tax_Amount { get; set; }
        public string? Fbr { get; set; }
        public CampaginStatus? Status { get; set; }
        public BudgetLimitTypeStatus? Budget_Limit_Type { get; set; }
        public int? Budget_Limit_Value { get; set; }
        public int Priority { get; set; }
        public List<CampaignBankDto> CampaignBanks { get; set; } = new();
    }

}
