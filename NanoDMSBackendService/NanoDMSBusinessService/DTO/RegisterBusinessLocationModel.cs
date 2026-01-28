namespace NanoDMSBusinessService.DTO
{
    public class RegisterBusinessLocationModel
    {
        public Guid BusinessId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public Guid Country { get; set; }
        public Guid City { get; set; }
        public Guid State { get; set; }
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
        // 👇 Multiple PSPs
        public List<Guid> Psp_Ids { get; set; } = new();

        // 👇 Multiple Banks
        public List<BankSettlementCreateDto> Banks { get; set; } = new();
    }
}
