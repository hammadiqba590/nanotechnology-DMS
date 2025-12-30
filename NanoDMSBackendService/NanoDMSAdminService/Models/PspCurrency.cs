using NanoDMSAdminService.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NanoDMSAdminService.Models
{
    public class PspCurrency : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        public Guid Psp_Id { get; set; }
        [ForeignKey(nameof(Psp_Id))]
        public Psp Psp { get; set; } = null!;

        public Guid Currency_Id { get; set; }

        [ForeignKey(nameof(Currency_Id))]
        public Currency Currency { get; set; } = null!;
    }

}
