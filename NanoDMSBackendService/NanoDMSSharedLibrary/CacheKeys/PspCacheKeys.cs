using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoDMSSharedLibrary.CacheKeys
{
    public class PspCacheKeys
    {
        public const string All = "psps:all";

        public static string ById(Guid id)
            => $"psps:{id}";

        public static string Paged(int page, int size,string name = "",string shortname = "", string code = "",string currencycode = "",string registrationnumber = "",string email = "",string phone = "",string apikey = "",string integrationtype = "",string compliancestatus = "",string settlement = "",string kyc = "")
            => $"psps:paged:{page}:{size}:{name}:{shortname}:{code}:{currencycode}:{registrationnumber}:{email}:{phone}:{apikey}:{integrationtype}:{compliancestatus}:{settlement}:{kyc}";
    }
}
