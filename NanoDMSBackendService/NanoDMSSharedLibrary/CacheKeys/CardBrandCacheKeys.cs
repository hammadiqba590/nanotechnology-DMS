using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoDMSSharedLibrary.CacheKeys
{
    public class CardBrandCacheKeys
    {
        public const string All = "cardbrands:all";

        public static string ById(Guid id)
            => $"cardbrands:{id}";

        public static string Paged(int page, int size)
            => $"cardbrands:paged:{page}:{size}";
    }
}
