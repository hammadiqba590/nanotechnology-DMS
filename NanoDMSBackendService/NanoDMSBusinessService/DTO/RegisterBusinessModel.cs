namespace NanoDMSBusinessService.DTO
{
    public class RegisterBusinessModel
    {
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public Guid TimeZoneId { get; set; }
        public Guid CurrencyId { get; set; }
        public Guid FinancialYearStartMonth { get; set; }
        public Guid StockAccountingMethod { get; set; }
        public IFormFile? Logo { get; set; } 
        public string Ntn { get; set; } = string.Empty;
        public string Stn { get; set; } = string.Empty;
        public string Tax3 { get; set; } = string.Empty;
        public string Tax4 { get; set; } = string.Empty;
    }
}
