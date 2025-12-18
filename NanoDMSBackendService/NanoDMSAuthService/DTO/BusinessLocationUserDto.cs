namespace NanoDMSAuthService.DTO
{
    public class BusinessLocationUserDto
    {
        public Guid Id { get; set; }
        public Guid BusinessLocationId { get; set; }
        public Guid BusinessId { get; set; }
        public Guid UserId { get; set; }
        public bool Deleted { get; set; }
    }
}
