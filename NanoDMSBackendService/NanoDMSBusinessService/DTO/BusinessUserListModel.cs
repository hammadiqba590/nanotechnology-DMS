using System.ComponentModel.DataAnnotations.Schema;
using NanoDMSBusinessService.Common;
using NanoDMSBusinessService.Models;

namespace NanoDMSBusinessService.DTO
{
    public class BusinessUserListModel : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid BusinessId { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
    }
}
