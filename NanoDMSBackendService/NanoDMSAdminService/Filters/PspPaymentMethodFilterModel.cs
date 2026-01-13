using NanoDMSAdminService.Blocks;
using NanoDMSAdminService.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace NanoDMSAdminService.Filters
{
    public class PspPaymentMethodFilterModel
    {
        public Guid? Psp_Id { get; set; }
        public PspPaymentTypeStatus? Payment_Type { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
