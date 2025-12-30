using NanoDMSBusinessService.Common;

namespace NanoDMSBusinessService.Models
{
    public class Business: BaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Start_Date { get; set; }
        public Guid Time_Zone_Id { get; set; }
        public Guid Currency_Id { get; set; }
        public Guid Financial_Year_Start_Month { get; set; }
        public Guid Stock_Accounting_Method { get; set; }
        public string Logo { get; set; } = string.Empty;
        public string Ntn { get; set; } = string.Empty;
        public string Stn { get; set; } = string.Empty;
        public string Tax3 { get; set; } = string.Empty;
        public string Tax4 { get; set;} = string.Empty;

        // Navigation property for BusinessUser
        public ICollection<BusinessLocation> BusinessLocations { get; set; } = new List<BusinessLocation>();
        public ICollection<BusinessUser> BusinessUsers { get; set; } = new List<BusinessUser>();
        public ICollection<BusinessConfig> BusinessConfigs { get; set; } = new List<BusinessConfig>();
        public ICollection<BusinessLocationUser> BusinessLocationUsers { get; set; } = new List<BusinessLocationUser>();
    }
}
