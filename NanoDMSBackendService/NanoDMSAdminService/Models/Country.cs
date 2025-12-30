using NanoDMSAdminService.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NanoDMSAdminService.Models
{
    public class Country : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(2)]
        public string Iso2 { get; set; } = string.Empty;

        [Required, StringLength(3)]
        public string Iso3 { get; set; } = string.Empty;

        [StringLength(3)]
        public string? Numeric_Code { get; set; }

        [MaxLength(10)]
        public string? Phone_Code { get; set; }

        [StringLength(3)]
        public string? Currency_Code { get; set; }

        [MaxLength(10)]
        public string? Currency_Symbol { get; set; }

        [MaxLength(10)]
        public string? Flag_Emoji { get; set; }

        public Guid Time_Zone { get; set; }

        public ICollection<Currency>? Currencies { get; set; } = new List<Currency>();

        public ICollection<CardBin>? Card_Bins { get; set; } = new List<CardBin>();

        public ICollection<Bank>? Banks { get; set; } = new List<Bank>();

        public ICollection<Psp>? Psps { get; set; } = new List<Psp>();

    }
}
