namespace NanoDMSBusinessService.DTO
{
    public class UpdateBankSettlementModel
    {
        public Guid Bank_Id { get; set; }
        public decimal Merchant_Share { get; set; }
        public decimal Bank_Share { get; set; }
        public decimal Tax_Value { get; set; }
        public bool Tax_On_Merchant { get; set; }
    }

}
