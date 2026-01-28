using NanoDMSAdminService.DTO.CardBin;

namespace NanoDMSAdminService.DTO.CardBrand
{
    public class CardBrandGroupDto
    {
        public Guid Card_Brand_Id { get; set; }
        public string Card_Brand_Name { get; set; } = "";
        public List<CardBinItemDto> CardBins { get; set; } = new();
    }
}
