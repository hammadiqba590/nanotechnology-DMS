using NanoDMSAdminService.Blocks;

namespace NanoDMSAdminService.DTO.CampaignBank
{
    public class CampaignBankUpdateDto : CampaignBankCreateDto
    {
        public RecordStatus Status { get; set; }
    }
}
