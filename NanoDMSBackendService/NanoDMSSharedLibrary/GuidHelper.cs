using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoDMSSharedLibrary
{
    public static class GuidHelper
    {
        public static Guid NewGuid()
        {
            return Guid.NewGuid();
        }

        public static bool IsValid(string guid)
        {
            return Guid.TryParse(guid, out _);
        }
    }

    #region Usage
    //if (!GuidHelper.IsValid(id))
   // return BadRequest("Invalid GUID");

    #endregion
}
