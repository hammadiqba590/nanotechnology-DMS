using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoDMSSharedLibrary.CacheKeys
{
    public class DiscountRuleHistoryCacheKeys
    {
        public const string All = "discountrulehistories:all";

        public static string ById(Guid id)
            => $"discountrulehistories:{id}";

        public static string Paged(int page, int size,string ruleid = "",string currencyid = "",string cardbinid = "",string discounttype = "",string paymenttype = "",string changetype = "")
            => $"discountrulehistories:paged:{page}:{size}:{ruleid}:{currencyid}:{cardbinid}:{discounttype}:{paymenttype}:{changetype}";
    }
}
