using NanoDMSAdminService.Common;
using System.ComponentModel.DataAnnotations;

namespace NanoDMSAdminService.DTO.Bank
{
    // DTOs/BankDto.cs

    public class BankDto : BaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Short_Name { get; set; } = string.Empty;
        public string Short_Code { get; set; } = string.Empty;
        public string? Swift_Code { get; set; }
        public Guid Country_Id { get; set; }
        public string Country_Name { get; set; } = string.Empty;
    }

    

}
