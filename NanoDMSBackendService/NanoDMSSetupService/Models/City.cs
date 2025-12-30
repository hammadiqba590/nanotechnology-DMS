using System.ComponentModel.DataAnnotations.Schema;
using NanoDMSSetupService.Common;

namespace NanoDMSSetupService.Models
{
    public class City : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid State_Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
