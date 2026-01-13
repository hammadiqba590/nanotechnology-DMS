using NanoDMSAdminService.Blocks;
using NanoDMSAdminService.Common;

namespace NanoDMSAdminService.DTO.CampaignCardBin
{
    public class CampaignCardBinDto : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid Campagin_Bank_Id { get; set; }
        public Guid Card_Bin_Id { get; set; }
        public string Bank_Name { get; set; } = "";
        public string Card_Bin { get; set; } = "";
        public CampaginCardBinStatus? Status { get; set; }
    }
}
