using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoDMSSharedLibrary.CacheKeys
{
    public class CampaignCacheKeys
    {
        public const string All = "campaigns:all";

        public static string ById(Guid id)
            => $"campaigns:{id}";

        public static string Paged(int page, int size,string campaignname = "",string status = "")
            => $"campaigns:paged:{page}:{size}:{campaignname}:{status}";
    }
}
