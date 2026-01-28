using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoDMSSharedLibrary
{
    public static class EnumHelper
    {
        public static List<EnumItemDto> GetEnum<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(e => new EnumItemDto
                {
                    Name = e.ToString(),
                    Value = Convert.ToInt32(e)
                })
                .ToList();
        }

        public class EnumItemDto
        {
            public string Name { get; set; } = default!;
            public int Value { get; set; }
        }

    }

}
