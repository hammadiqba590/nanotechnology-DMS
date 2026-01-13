using System.ComponentModel.DataAnnotations;

namespace NanoDMSAdminService.DTO.PspCategory
{
    public class PspCategoryCreateDto
    {
        [Required, MaxLength(50)]
        public string Name { get; set; } = null!;
        [MaxLength(255)]
        public string? Description { get; set; }
        public Guid Business_Id { get; set; }
        public Guid Business_Location_Id { get; set; }
    }
}
