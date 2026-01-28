namespace NanoDMSBusinessService.DTO
{
    public class UpdateBusinessLocationModel
    {
        public string Id { get; set; } = string.Empty;
        public string BusinessId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public int DiscountBeforeTax { get; set; }
        public decimal PosCharge { get; set; }
        public int DiscountBeforePosCharge { get; set; }
        public decimal ServiceCharges { get; set; }
        public int DiscountBeforeServiceCharge { get; set; }
        public List<Guid> Psp_Ids { get; set; } = new();
        public List<UpdateBankSettlementModel> Banks { get; set; } = new();

    }
}
