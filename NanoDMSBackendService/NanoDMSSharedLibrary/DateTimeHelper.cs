using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoDMSSharedLibrary
{
    public static class DateTimeHelper
    {
        public static DateTime UtcNow()
        {
            return DateTime.UtcNow;
        }

        public static DateTime PakistanNow()
        {
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(
                DateTime.UtcNow, "Pakistan Standard Time");
        }
    }
    #region Usage
    // entity.Create_Date = DateTimeHelper.UtcNow();

    #endregion
}
