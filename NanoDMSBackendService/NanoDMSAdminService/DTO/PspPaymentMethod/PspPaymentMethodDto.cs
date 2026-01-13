using NanoDMSAdminService.Blocks;
using NanoDMSAdminService.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace NanoDMSAdminService.DTO.PspPaymentMethod
{
    public class PspPaymentMethodDto : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid Psp_Id { get; set; }
        public string? Psp_Name { get; set; }
        public PspPaymentTypeStatus? Payment_Type { get; set; }
    }
}
