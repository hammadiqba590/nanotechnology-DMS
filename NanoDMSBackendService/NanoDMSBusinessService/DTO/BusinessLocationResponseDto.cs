namespace NanoDMSBusinessService.DTO
{
    public class BusinessLocationResponseDto
    {
        public Guid Id { get; set; }
        public Guid BusinessId { get; set; }
        public string Name { get; set; } = "";
        public string Address { get; set; } = "";
        public Guid Country { get; set; }
        public Guid State { get; set; }
        public Guid City { get; set; }
        public string PostalCode { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Mobile { get; set; } = "";
        public string Email { get; set; } = "";
        public string? Website { get; set; }

        public decimal DiscountBeforeTax { get; set; }
        public decimal PosCharge { get; set; }
        public decimal DiscountBeforePosCharge { get; set; }
        public decimal ServiceCharges { get; set; }
        public decimal DiscountBeforeServiceCharge { get; set; }

        public List<PspDto> LocationPsps { get; set; } = new();
        public List<BankSettlementDto> LocationBanks { get; set; } = new();
    }

}
