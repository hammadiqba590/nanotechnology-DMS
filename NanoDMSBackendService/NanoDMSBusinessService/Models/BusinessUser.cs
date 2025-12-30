using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using NanoDMSBusinessService.Common;

namespace NanoDMSBusinessService.Models
{
    public class BusinessUser: BaseEntity
    {
        public Guid Id { get; set; }
        public Guid Business_Id { get; set; }

        [ForeignKey("Business_Id")]
        public Business? Business { get; set; }
        public Guid User_Id { get; set; } 

        
    }
}
