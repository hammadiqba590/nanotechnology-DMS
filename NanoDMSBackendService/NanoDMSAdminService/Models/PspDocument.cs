using NanoDMSAdminService.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NanoDMSAdminService.Models
{
    public class PspDocument : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        public Guid Psp_Id { get; set; }
        [ForeignKey(nameof(Psp_Id))]
        public Psp Psp { get; set; } = null!;

        [Required, MaxLength(50)]
        public string Doc_Type { get; set; } = null!; //'license, MoU, KYC, other',

        [Required, MaxLength(255)]
        public string Doc_Url { get; set; } = null!;
 
    }

}
