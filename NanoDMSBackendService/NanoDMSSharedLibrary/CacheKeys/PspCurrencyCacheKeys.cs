using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoDMSSharedLibrary.CacheKeys
{
    public class PspCurrencyCacheKeys
    {
        public const string All = "pspcurrencies:all";

        public static string ById(Guid id)
            => $"pspcurrencies:{id}";

        public static string Paged(int page, int size)
            => $"pspcurrencies:paged:{page}:{size}";
    }
}

