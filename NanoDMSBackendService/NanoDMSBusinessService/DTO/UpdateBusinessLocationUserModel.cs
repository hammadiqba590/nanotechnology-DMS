namespace NanoDMSBusinessService.DTO
{
    public class UpdateBusinessLocationUserModel
    {
        public Guid Id { get; set; }
        public Guid BusinessId { get; set; }
        public List<Guid>? BusinessLocationIds { get; set; }
        public Guid UserId { get; set; }
    }
}
