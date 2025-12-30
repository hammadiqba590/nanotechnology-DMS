using NanoDMSAdminService.Blocks;
using NanoDMSAdminService.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NanoDMSAdminService.Models
{
    public class PspPaymentMethod : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        public Guid Psp_Id { get; set; }
        [ForeignKey(nameof(Psp_Id))]
        public Psp Psp { get; set; } = null!;
        public PspPaymentTypeStatus? Payment_Type { get; set; } // card','wallet','bank_transfer','upi','qr'
    }
}
