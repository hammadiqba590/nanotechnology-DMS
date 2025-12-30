using NanoDMSAdminService.Common;
using System.ComponentModel.DataAnnotations;

namespace NanoDMSAdminService.DTO.Bank
{
    public class BankCreateDto 
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        [Required, MaxLength(50)]
        public string Short_Name { get; set; } = string.Empty;
        [Required, MaxLength(20)]
        public string Short_Code { get; set; } = string.Empty;
        public string? Swift_Code { get; set; }
        [Required]
        public Guid Country_Id { get; set; }
        [Required]
        public Guid Business_Id { get; set; }
        [Required]
        public Guid BusinessLocation_Id { get; set; }

    }
}
