namespace NanoDMSSetupService.DTO
{
    public class UpdateCurrencyModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal ConversionRateToUSD { get; set; }
    }
}
