using NanoDMSAdminService.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace NanoDMSAdminService.DTO.PspCurrency
{
    public class PspCurrencyDto: BaseEntity
    {
        public Guid Id { get; set; }
        public Guid Psp_Id { get; set; }
        public string? Psp_Name { get; set; }
        public Guid Currency_Id { get; set; }
        public string? Currency_Name { get; set; }
    }
}
