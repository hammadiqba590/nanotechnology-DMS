using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoDMSSharedLibrary.CacheKeys
{
    public class PosTerminalStatusHistoryCacheKeys
    {
        public const string All = "posterminalstatushistories:all";

        public static string ById(Guid id)
            => $"posterminalstatushistories:{id}";

        public static string Paged(int page, int size)
            => $"posterminalstatushistories:paged:{page}:{size}";
    }
}
