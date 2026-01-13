using NanoDMSAdminService.Blocks;

namespace NanoDMSAdminService.Filters
{
    public class CampaignCardBinFilterModel
    {
        public CampaginCardBinStatus? Status { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
