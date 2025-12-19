namespace NanoDMSBusinessService.DTO
{
    public class BusinessFilterModel
    {
        public string? Name { get; set; } = null;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public Guid BusinessId { get; set; }
        public Guid UserId { get; set; }
    }
}
