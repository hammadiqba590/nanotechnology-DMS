using NanoDMSAdminService.Blocks;
using NanoDMSAdminService.DTO.CardBrand;

namespace NanoDMSAdminService.DTO.CardBin
{
    public class CardBinGroupDto
    {
        public Guid Bank_Id { get; set; }
        public string Bank_Name { get; set; } = "";
        public List<CardBrandGroupDto> Brands { get; set; } = new();
    }

    public class CardBinItemDto
    {
        public Guid Id { get; set; }
        public string Card_Bin_Value { get; set; } = "";
        public LocalInternationalStatus? Local_International { get; set; }
        public string Products { get; set; } = "";
    }
}
