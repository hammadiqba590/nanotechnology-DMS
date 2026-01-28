using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoDMSSharedLibrary.CacheKeys
{
    public class DiscountRuleCacheKeys
    {
        public const string All = "discountrules:all";

        public static string ById(Guid id)
            => $"discountrules:{id}";

        public static string Paged(int page, int size,string cardbinid = "",string discounttype = "",string paymenttype = "",string limittype = "")
            => $"discountrules:paged:{page}:{size}:{cardbinid}:{discounttype}:{paymenttype}:{limittype}";
    }
}
