using NanoDMSAdminService.Blocks;

namespace NanoDMSAdminService.DTO.CardBin
{
    public class CardBinLookupDto
    {
        public Guid Id { get; set; }
        public string Card_Bin_Value { get; set; } = "";
        public Guid Bank_Id { get; set; }
        public Guid Card_Brand_Id { get; set; }
        public LocalInternationalStatus? Local_International { get; set; }

        // 🔥 Concatenated value
        public string Products { get; set; } = "";
    }

}
