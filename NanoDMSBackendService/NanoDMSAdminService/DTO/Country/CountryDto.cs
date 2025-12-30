using NanoDMSAdminService.Common;

namespace NanoDMSAdminService.DTO.Country
{
    public class CountryDto : BaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string Iso2 { get; set; } = "";
        public string Iso3 { get; set; } = "";
        public string? Numeric_Code { get; set; }
        public string? Phone_Code { get; set; }
        public string? Currency_Code { get; set; }
        public string? Currency_Symbol { get; set; }
        public string? Flag_Emoji { get; set; }
        public Guid Time_Zone { get; set; }
    }

}
