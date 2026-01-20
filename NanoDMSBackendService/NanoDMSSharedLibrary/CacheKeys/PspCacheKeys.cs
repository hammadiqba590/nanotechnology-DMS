using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoDMSSharedLibrary.CacheKeys
{
    public class PspCacheKeys
    {
        public const string All = "psps:all";

        public static string ById(Guid id)
            => $"psps:{id}";

        public static string Paged(int page, int size)
            => $"psps:paged:{page}:{size}";
    }
}
