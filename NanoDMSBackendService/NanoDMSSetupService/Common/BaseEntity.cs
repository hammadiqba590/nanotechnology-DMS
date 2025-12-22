namespace NanoDMSSetupService.Common
{
    public abstract  class BaseEntity
    {
        public bool Deleted { get; set; } = false;
        public bool Published { get; set; } = false;
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
        public Guid CreateUser { get; set; }
        public DateTime LastUpdateDate { get; set; } = DateTime.UtcNow;
        public Guid LastUpdateUser { get; set; }
    }
}
