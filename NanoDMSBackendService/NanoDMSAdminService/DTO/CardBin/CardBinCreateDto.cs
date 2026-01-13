using NanoDMSAdminService.Blocks;
using System.ComponentModel.DataAnnotations;

namespace NanoDMSAdminService.DTO.CardBin
{
    public class CardBinCreateDto
    {
        [Required]
        public Guid Bank_Id { get; set; }
        [Required, MaxLength(12)]
        public string Card_Bin_Value { get; set; } = "";
        [Required]
        public Guid Card_Brand_Id { get; set; }
        [Required]
        public Guid Card_Type_Id { get; set; }
        [Required]
        public Guid? Card_Level_Id { get; set; }
        public LocalInternationalStatus? Local_International { get; set; }
        public Guid? Country_Id { get; set; }
        public string? Country_Name { get; set; }
        public Guid Business_Id { get; set; }
        public Guid Business_Location_Id { get; set; }
    }
}
