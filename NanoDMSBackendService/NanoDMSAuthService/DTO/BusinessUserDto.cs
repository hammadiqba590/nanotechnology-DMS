namespace NanoDMSAuthService.DTO
{
    public class BusinessUserDto
    {
        public Guid Id { get; set; }
        public Guid BusinessId { get; set; }
        public Guid UserId { get; set; }
        public bool Deleted { get; set; }
    }
}
