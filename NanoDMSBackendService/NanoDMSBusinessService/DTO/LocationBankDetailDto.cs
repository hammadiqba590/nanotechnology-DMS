namespace NanoDMSBusinessService.DTO
{
    public class LocationBankDetailDto
    {
        public Guid BankId { get; set; }
        public string BankName { get; set; } = string.Empty;
        public decimal MerchantShare { get; set; }
        public decimal BankShare { get; set; }
        public decimal TaxValue { get; set; }
        public bool TaxOnMerchant { get; set; }
    }

}
