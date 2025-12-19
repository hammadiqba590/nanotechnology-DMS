namespace NanoDMSBusinessService.DTO
{
    public record BusinessLocationsDto
    {
        public Guid Id { get; set; }
        public Guid BusinessId { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public Guid Country { get; set; }
        public Guid State { get; set; }
        public Guid City { get; set; }
        public string PostalCode { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public bool Deleted { get; set; } = false;
        public bool Published { get; set; } = false;
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
        public Guid CreateUser { get; set; }
        public DateTime LastUpdateDate { get; set; } = DateTime.UtcNow;
        public Guid LastUpdateUser { get; set; }
    }
}
