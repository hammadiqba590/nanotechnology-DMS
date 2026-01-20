using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoDMSSharedLibrary.CacheKeys
{
    public static class BankCacheKeys
    {
        public const string All = "banks:all";

        public static string ById(Guid id)
            => $"banks:{id}";

        public static string Paged(int page, int size)
            => $"banks:paged:{page}:{size}";
    }

}
