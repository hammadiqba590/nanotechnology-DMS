using NanoDMSAdminService.Common;

namespace NanoDMSAdminService.DTO.PspCategory
{
    public class PspCategoryDto : BaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}
