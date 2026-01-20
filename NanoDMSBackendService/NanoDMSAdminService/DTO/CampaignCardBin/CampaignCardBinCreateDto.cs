using NanoDMSAdminService.Blocks;
using NanoDMSAdminService.DTO.DiscountRule;
using System.ComponentModel.DataAnnotations;

namespace NanoDMSAdminService.DTO.CampaignCardBin
{
    public class CampaignCardBinCreateDto
    {
        [Required]
        public Guid Campagin_Bank_Id { get; set; }
        [Required]
        public Guid Card_Bin_Id { get; set; }

        public CampaginCardBinStatus? Status { get; set; }
        [Required]
        public Guid Business_Id { get; set; }

        [Required]
        public Guid Business_Location_Id { get; set; }

        public List<DiscountRuleCreateDto> DiscountRules { get; set; } = [];
    }
}
