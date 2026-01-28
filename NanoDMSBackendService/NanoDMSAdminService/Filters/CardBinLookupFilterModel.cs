using NanoDMSAdminService.Blocks;

namespace NanoDMSAdminService.Filters
{
    public class CardBinLookupFilterModel
    {
        public List<Guid>? Bank_Ids { get; set; }
        public List<Guid>? Card_Brand_Ids { get; set; }
        public List<LocalInternationalStatus>? Local_International_List { get; set; }
    }

}
