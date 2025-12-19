namespace NanoDMSBusinessService.DTO
{
    public class RegisterBusinessLocationUserModel
    {
        public Guid BusinessId { get; set; }
        public List<Guid>? BusinessLocationIds { get; set; } // Change from single to multiple
        public Guid UserId { get; set; }
    }

}
