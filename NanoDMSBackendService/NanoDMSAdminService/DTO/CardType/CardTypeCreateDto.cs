using System.ComponentModel.DataAnnotations;

namespace NanoDMSAdminService.DTO.CardType
{
    public class CardTypeCreateDto
    {
        [Required]
        public string Name { get; set; } = "";
        public Guid Business_Id { get; set; }
        public Guid Business_Location_Id { get; set; }
    }
}
