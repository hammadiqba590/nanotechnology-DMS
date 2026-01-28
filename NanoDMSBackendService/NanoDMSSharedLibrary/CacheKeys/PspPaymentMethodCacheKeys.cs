using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoDMSSharedLibrary.CacheKeys
{
    public class PspPaymentMethodCacheKeys
    {
        public const string All = "psppaymentmethods:all";

        public static string ById(Guid id)
            => $"psppaymentmethods:{id}";

        public static string Paged(int page, int size,string pspid = "",string paymenttype = "")
            => $"psppaymentmethods:paged:{page}:{size}:{pspid}:{paymenttype}";
    }
}
