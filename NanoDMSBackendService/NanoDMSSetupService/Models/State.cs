using System.ComponentModel.DataAnnotations.Schema;
using NanoDMSSetupService.Common;

namespace NanoDMSSetupService.Models
{
    public class State : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid Country_Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
