using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoDMSSharedLibrary.CacheKeys
{
    public class PosTerminalAssignmentCacheKeys
    {
        public const string All = "posterminalassignments:all";

        public static string ById(Guid id)
            => $"posterminalassignments:{id}";

        public static string Paged(int page, int size,string terminalid = "",string mid = "",string tid = "")
            => $"posterminalassignments:paged:{page}:{size}:{terminalid}:{mid}:{tid}";
    }
}
