using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoDMSSharedLibrary.CacheKeys
{
    public class PosTerminalMasterCacheKeys
    {
        public const string All = "posterminalmasters:all";

        public static string ById(Guid id)
            => $"posterminalmasters:{id}";

        public static string Paged(int page, int size)
            => $"posterminalmasters:paged:{page}:{size}";
    }
}
