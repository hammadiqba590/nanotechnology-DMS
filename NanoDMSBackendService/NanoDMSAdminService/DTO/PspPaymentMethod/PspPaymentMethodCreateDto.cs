using NanoDMSAdminService.Blocks;
using System.ComponentModel.DataAnnotations;

namespace NanoDMSAdminService.DTO.PspPaymentMethod
{
    public class PspPaymentMethodCreateDto
    {
        [Required]
        public Guid Psp_Id { get; set; }
        public PspPaymentTypeStatus? Payment_Type { get; set; }
        public Guid Business_Id { get; set; }
        public Guid Business_Location_Id { get; set; }
    }
}
