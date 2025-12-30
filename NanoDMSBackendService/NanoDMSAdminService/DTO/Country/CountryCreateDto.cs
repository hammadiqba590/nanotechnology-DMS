using NanoDMSAdminService.Common;
using System.ComponentModel.DataAnnotations;

namespace NanoDMSAdminService.DTO.Country
{
    public class CountryCreateDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = "";
        [Required, StringLength(2)]
        public string Iso2 { get; set; } = "";
        [Required, StringLength(3)]
        public string Iso3 { get; set; } = "";
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
        [Required]
        public Guid Business_Id { get; set; }
        [Required]
        public Guid BusinessLocation_Id { get; set; }
    }

}
