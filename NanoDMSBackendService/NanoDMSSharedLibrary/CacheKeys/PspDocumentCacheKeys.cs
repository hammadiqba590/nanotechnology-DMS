using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoDMSSharedLibrary.CacheKeys
{
    public class PspDocumentCacheKeys
    {
        public const string All = "pspdocuments:all";

        public static string ById(Guid id)
            => $"pspdocuments:{id}";

        public static string Paged(int page, int size)
            => $"pspdocuments:paged:{page}:{size}";
    }
}
