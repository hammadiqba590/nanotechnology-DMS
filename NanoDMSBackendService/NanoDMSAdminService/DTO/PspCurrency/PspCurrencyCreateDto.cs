using System.ComponentModel.DataAnnotations;

namespace NanoDMSAdminService.DTO.PspCurrency
{
    public class PspCurrencyCreateDto
    {
        [Required]
        public Guid Psp_Id { get; set; }
        [Required]
        public Guid Currency_Id { get; set; }
        public Guid Business_Id { get; set; }
        public Guid Business_Location_Id { get; set; }
    }
}
