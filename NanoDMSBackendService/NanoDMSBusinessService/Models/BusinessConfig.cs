using System.ComponentModel.DataAnnotations.Schema;
using NanoDMSBusinessService.Common;

namespace NanoDMSBusinessService.Models
{
    public class BusinessConfig: BaseEntity
    {
        public Guid Id { get; set; }
        public string Name_Key { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Config_Value { get; set; } = string.Empty;
        public string Config_Type { get; set; } = string.Empty;
        public Guid Business_Id { get; set; }

        [ForeignKey("Business_Id")]
        public Business? Business { get; set; }

        public Guid Business_Location_Id { get; set; }

        [ForeignKey("Business_Location_Id")]
        public BusinessLocation? BusinessLocation { get; set; }

    }
}
