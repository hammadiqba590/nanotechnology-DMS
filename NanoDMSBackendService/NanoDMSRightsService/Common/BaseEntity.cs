using NanoDMSRightsService.Blocks;

namespace NanoDMSRightsService.Common
{
    public abstract class BaseEntity
    {
        public bool Deleted { get; set; } = false;
        public bool Published { get; set; } = false;
        public DateTime Create_Date { get; set; } = DateTime.UtcNow;
        public Guid Create_User { get; set; }
        public DateTime Last_Update_Date { get; set; } = DateTime.UtcNow;
        public Guid Last_Update_User { get; set; }
        public Guid Business_Id { get; set; }
        public Guid BusinessLocation_Id { get; set; }
        public bool Is_Active { get; set; } = true;
        public DateTime Start_Date { get; set; }
        public DateTime End_Date { get; set; }
        public RecordStatus RecordStatus { get; set; } = RecordStatus.Active;
    }
}
