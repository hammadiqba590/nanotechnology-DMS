using NanoDMSBusinessService.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace NanoDMSBusinessService.Models
{
    public class BusinessLocationBankSettlement : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid Business_Location_Id { get; set; }
        [ForeignKey(nameof(Business_Location_Id))]
        public BusinessLocation? Business_Location { get; set; }
        public Guid Bank_Id { get; set; }
        public decimal Merchant_Share { get; set; }   // %
        public decimal Bank_Share { get; set; }       // %
        public decimal Tax_Value { get; set; }        // %
        public int Tax_On_Merchant { get; set; }       // 1 = Yes, 0 = No
    }

}
