using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoDMSSharedLibrary.CacheKeys
{
    public class CampaignBankCacheKeys
    {
        public const string All = "campaignbanks:all";

        public static string ById(Guid id)
            => $"campaignbanks:{id}";

        public static string Paged(int page, int size, string tax = "",
        string budgetType = "",
        string discountMode = "",
        string status = "")
            => $"campaignbanks:paged:{page}:{size}:{tax}:{budgetType}:{discountMode}:{status}";
    }
}
