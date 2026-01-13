using NanoDMSAdminService.Blocks;

namespace NanoDMSAdminService.Filters
{
    public class CampaignFilterModel
    {
        public string? Campaign_Name { get; set; }
        public CampaginStatus? Status { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

}
