using NanoDMSAdminService.Blocks;

namespace NanoDMSAdminService.DTO.Campagin
{
    public class CampaignUpdateDto : CampaignCreateDto
    {
        public CampaginStatus? Status { get; set; }
    }

}
