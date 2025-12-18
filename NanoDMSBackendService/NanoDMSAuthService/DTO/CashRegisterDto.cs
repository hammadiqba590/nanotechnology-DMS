namespace NanoDMSAuthService.DTO
{
    public class CashRegisterDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public bool Deleted { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
