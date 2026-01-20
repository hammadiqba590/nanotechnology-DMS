using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoDMSSharedLibrary.CacheKeys
{
    public class CardTypeCacheKeys
    {
        public const string All = "cardtypes:all";

        public static string ById(Guid id)
            => $"cardtypes:{id}";

        public static string Paged(int page, int size)
            => $"cardtypes:paged:{page}:{size}";
    }
}
