using NanoDMSAdminService.Common;

namespace NanoDMSAdminService.DTO.CardLevel
{
    public class CardLevelDto : BaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
    }
}
