using NanoDMSBusinessService.Common;
using NanoDMSBusinessService.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace NanoDMSBusinessService.DTO
{
    public class BusinessLocationUserListModel:BaseEntity
    {
        public Guid Id { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public string? UserName { get; set; } = string.Empty;
        public string BusinessLocationName { get; set; } = string.Empty;
    }
}
