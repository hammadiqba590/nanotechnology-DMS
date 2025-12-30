using NanoDMSAdminService.Common;

namespace NanoDMSAdminService.DTO.Currency
{
    public class CurrencyDto : BaseEntity
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public string? Symbol { get; set; }

        public Guid? Country_Id { get; set; }
        public string? Country_Name { get; set; }
    }

}
