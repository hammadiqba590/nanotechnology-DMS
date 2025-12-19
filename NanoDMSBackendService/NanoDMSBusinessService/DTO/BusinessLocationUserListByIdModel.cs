using NanoDMSBusinessService.Common;

namespace NanoDMSBusinessService.DTO
{
    public class BusinessLocationUserListByIdModel
    {
        public Guid Id { get; set; }
        public Guid BusinessId { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public List<BusinessLocationDto> ? BusinessLocations { get; set; }
        public Guid UserId { get; set; }
    }
    public class BusinessLocationDto : BaseEntity
    {
        public Guid BusinessLocationId { get; set; }
        public string BusinessLocationName { get; set; } =string.Empty;
    }
}
