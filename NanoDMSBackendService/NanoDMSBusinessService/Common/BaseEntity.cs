namespace NanoDMSBusinessService.Common
{
    public abstract class BaseEntity
    {
        public bool Deleted { get; set; } = false;
        public bool Published { get; set; } = false;
        public DateTime Create_Date { get; set; } = DateTime.UtcNow;
        public Guid Create_User { get; set; }
        public DateTime Last_Update_Date { get; set; } = DateTime.UtcNow;
        public Guid Last_Update_User { get; set; }
    }
}
