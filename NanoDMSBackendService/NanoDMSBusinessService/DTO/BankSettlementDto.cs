namespace NanoDMSBusinessService.DTO
{
    public class BankSettlementDto
    {
        public Guid Id { get; set; }
        public Guid Bank_Id { get; set; }
        public decimal Merchant_Share { get; set; }
        public decimal Bank_Share { get; set; }
        public decimal Tax_Value { get; set; }
        public int TaxOnMerchant { get; set; }
    }
}
