using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoDMSSharedLibrary.CacheKeys
{
    public class CardBinCacheKeys
    {
        public const string All = "cardbins:all";

        public static string ById(Guid id)
            => $"cardbins:{id}";

        public static string Paged(int page, int size, string binValue = "",
        string bankId = "",
        string brandId = "",
        string typeId = "",
        string levelId = "",
        string countryId = "",
        string localIntl = "",
        string isActive = "")
            => $"cardbins:paged:{page}:{size}:{bankId}:{brandId}:{typeId}:{levelId}:{countryId}:{localIntl}:{isActive}";
    }
}
