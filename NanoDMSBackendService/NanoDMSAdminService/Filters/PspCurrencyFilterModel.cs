using NanoDMSAdminService.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace NanoDMSAdminService.Filters
{
    public class PspCurrencyFilterModel
    {
        public Guid? Psp_Id { get; set; }
        public Guid? Currency_Id { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
