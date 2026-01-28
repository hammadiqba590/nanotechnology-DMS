using NanoDMSAdminService.Blocks;
using NanoDMSAdminService.Common;

namespace NanoDMSAdminService.DTO.CardBin
{
    public class CardBinDto : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid Bank_Id { get; set; }
        public string? Bank_Name { get; set; } 
        public string Card_Bin_Value { get; set; } = "";
        public Guid Card_Brand_Id { get; set; }
        public string? Card_Brand_Name { get; set; }
        public Guid Card_Type_Id { get; set; }
        public string? Card_Type_Name { get; set; }
        public Guid? Card_Level_Id { get; set; }
        public string? Card_Level_Name { get; set; }
        public LocalInternationalStatus? Local_International { get; set; }
        public Guid? Country_Id { get; set; }
        public string? Country_Name { get; set; }
        public bool Is_Virtual { get; set; }

    }

}
