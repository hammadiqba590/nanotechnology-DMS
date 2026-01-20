using NanoDMSAdminService.Blocks;
using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.DiscountRule;

namespace NanoDMSAdminService.DTO.CampaignCardBin
{
    public class CampaignCardBinResponseDto : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid Campagin_Id { get; set; }
        public string Campaign_Name { get; set; } = "";
        public Guid Campagin_Bank_Id { get; set; }
        public string Bank_Name { get; set; } = "";
        public Guid Card_Bin_Id { get; set; }
        public CampaginCardBinStatus? Status { get; set; }
        public List<DiscountRuleResponseDto> DiscountRules { get; set; } = [];
    }
}
