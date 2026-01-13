using NanoDMSAdminService.Common;

namespace NanoDMSAdminService.DTO.CardBrand
{
    public class CardBrandDto : BaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
    }

}
