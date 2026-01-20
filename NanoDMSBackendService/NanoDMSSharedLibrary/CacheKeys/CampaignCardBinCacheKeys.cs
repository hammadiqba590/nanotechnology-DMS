using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoDMSSharedLibrary.CacheKeys
{
    public class CampaignCardBinCacheKeys
    {
        public const string All = "campaigncardbins:all";

        public static string ById(Guid id)
            => $"campaigncardbins:{id}";

        public static string Paged(int page, int size)
            => $"campaigncardbins:paged:{page}:{size}";
    }
}
