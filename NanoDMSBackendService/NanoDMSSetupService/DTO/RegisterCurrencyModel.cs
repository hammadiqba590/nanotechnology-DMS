namespace NanoDMSSetupService.DTO
{
    public class RegisterCurrencyModel
    {
        public string Name { get; set; } = string.Empty;
        public decimal ConversionRateToUSD { get; set; }
    }
}
