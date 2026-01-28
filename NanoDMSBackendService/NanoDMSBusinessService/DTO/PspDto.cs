using NanoDMSBusinessService.Common;
using NanoDMSBusinessService.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace NanoDMSBusinessService.DTO
{
    public class PspDto : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid PspId { get; set; }
    }
}
