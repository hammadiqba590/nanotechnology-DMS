using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoDMSSharedLibrary.CacheKeys
{
    public class PosTerminalConfigurationCacheKeys
    {
        public const string All = "posterminalconfigurations:all";

        public static string ById(Guid id)
            => $"posterminalconfigurations:{id}";

        public static string Paged(int page, int size)
            => $"posterminalconfigurations:paged:{page}:{size}";
    }
}
