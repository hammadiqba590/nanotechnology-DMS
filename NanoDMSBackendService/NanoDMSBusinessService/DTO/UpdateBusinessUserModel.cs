namespace NanoDMSBusinessService.DTO
{
    public class UpdateBusinessUserModel
    {
        public Guid Id { get; set; }
        public Guid BusinessId { get; set; }

        public Guid UserId { get; set; }
    }
}
