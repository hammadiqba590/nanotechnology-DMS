using NanoDMSAdminService.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NanoDMSAdminService.Models
{
    public class Currency : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required, StringLength(3)]
        public string Code { get; set; } = null!;

        [Required, MaxLength(50)]
        public string Name { get; set; } = null!;

        [MaxLength(10)]
        public string? Symbol { get; set; }
        public Guid? Country_Id { get; set; }
        [ForeignKey(nameof(Country_Id))]
        public Country? Country { get; set; }

        public ICollection<PspCurrency>? Psp_Currencies { get; set; } = new List<PspCurrency>();
    }
}
