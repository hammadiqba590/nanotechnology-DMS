using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoDMSSharedLibrary.CacheKeys
{
    public class PspCategoryCacheKeys
    {
        public const string All = "pspcategories:all";

        public static string ById(Guid id)
            => $"pspcategories:{id}";

        public static string Paged(int page, int size,string name = "")
            => $"pspcategories:paged:{page}:{size}:{name}";
    }
}
