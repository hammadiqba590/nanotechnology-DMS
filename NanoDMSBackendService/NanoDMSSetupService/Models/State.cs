using System.ComponentModel.DataAnnotations.Schema;
using NanoDMSSetupService.Common;

namespace NanoDMSSetupService.Models
{
    public class State : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid CountryId { get; set; }

        [ForeignKey("CountryId")] // Explicitly define the foreign key relationship
        public Country? Country { get; set; } // Navigation property to Country
        public string Name { get; set; } = string.Empty;

        // Navigation property for related cities
        public ICollection<City> Cities { get; set; } = new List<City>();
    }
}
