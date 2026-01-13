using NanoDMSAdminService.Common;

namespace NanoDMSAdminService.DTO.CardType
{
    public class CardTypeDto : BaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
    }
}
