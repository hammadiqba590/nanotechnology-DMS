using NanoDMSBusinessService.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace NanoDMSBusinessService.Models
{
    public class BusinessLocationPsp : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid Business_Location_Id { get; set; }
        [ForeignKey(nameof(Business_Location_Id))]
        public BusinessLocation? Business_Location { get; set; }

        public Guid Psp_Id { get; set; }   // Acquirer / PSP Id
    }

}
