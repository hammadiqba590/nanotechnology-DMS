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

        public static string Paged(int page, int size)
            => $"discountrulehistories:paged:{page}:{size}";
    }
}
