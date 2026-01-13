using NanoDMSAdminService.Blocks;

namespace NanoDMSAdminService.Filters
{
    public class CardBinFilterModel
    {
        public string? Card_Bin_Value { get; set; }
        public Guid? Bank_Id { get; set; }
        public Guid? Card_Brand_Id { get; set; }
        public Guid? Card_Type_Id { get; set; }
        public Guid? Card_Level_Id { get; set; }
        public Guid? Country_Id { get; set; }
        public LocalInternationalStatus? Local_International { get; set; }
        public bool? Is_Active { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
