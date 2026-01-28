using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoDMSSharedLibrary.CacheKeys
{
    public class CardLevelCacheKeys
    {
        public const string All = "cardlevels:all";

        public static string ById(Guid id)
            => $"cardlevels:{id}";

        public static string Paged(int page, int size, string name = "")
            => $"cardlevels:paged:{page}:{size}:{name}";
    }
}
