using System.ComponentModel.DataAnnotations;

namespace NanoDMSAdminService.DTO.CardBrand
{
    public class CardBrandCreateDto
    {
        [Required]
        public string Name { get; set; } = "";
        public Guid Business_Id { get; set; }
        public Guid Business_Location_Id { get; set; }
    }

}
