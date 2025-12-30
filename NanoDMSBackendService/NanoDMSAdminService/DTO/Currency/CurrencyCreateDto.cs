using NanoDMSAdminService.Common;
using System.ComponentModel.DataAnnotations;

namespace NanoDMSAdminService.DTO.Currency
{
    public class CurrencyCreateDto
    {
        [Required, StringLength(3)]
        public string Code { get; set; } = "";
        [Required, MaxLength(50)]
        public string Name { get; set; } = "";
        [MaxLength(10)]
        public string? Symbol { get; set; }
        [Required]
        public Guid? Country_Id { get; set; }

        [Required]
        public Guid Business_Id { get; set; }
        [Required]
        public Guid BusinessLocation_Id { get; set; }

    }

}
