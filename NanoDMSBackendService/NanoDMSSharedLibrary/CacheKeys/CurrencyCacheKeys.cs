using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoDMSSharedLibrary.CacheKeys
{
    public class CurrencyCacheKeys
    {
        public const string All = "currencies:all";

        public static string ById(Guid id)
            => $"currencies:{id}";

        public static string Paged(int page, int size)
            => $"currencies:paged:{page}:{size}";
    }
}
