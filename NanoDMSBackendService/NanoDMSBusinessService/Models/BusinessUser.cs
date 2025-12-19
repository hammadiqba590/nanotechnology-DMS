using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using NanoDMSBusinessService.Common;

namespace NanoDMSBusinessService.Models
{
    public class BusinessUser: BaseEntity
    {
        public Guid Id { get; set; }
        public Guid BusinessId { get; set; }

        [ForeignKey("BusinessId")]
        public Business? Business { get; set; }
        public Guid UserId { get; set; } 

        
    }
}
