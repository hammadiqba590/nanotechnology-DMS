using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoDMSSharedLibrary.CacheKeys
{
    public class CountryCacheKeys
    {
        public const string All = "countries:all";

        public static string ById(Guid id)
            => $"countries:{id}";

        public static string Paged(int page, int size,string name = "",string iso2 = "")
            => $"countries:paged:{page}:{size}:{name}:{iso2}";
    }
}
