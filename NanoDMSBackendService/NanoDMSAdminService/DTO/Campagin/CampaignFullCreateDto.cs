using NanoDMSAdminService.Blocks;
using NanoDMSAdminService.DTO.CampaignBank;
using System.ComponentModel.DataAnnotations;

namespace NanoDMSAdminService.DTO.Campagin
{
    public class CampaignFullCreateDto
    {
        [Required]
        public string Campaign_Name { get; set; } = "";
        public string? Description { get; set; }
        [Required]
        public Guid Psp_Id { get; set; }
        [Required]
        public Guid Currency_Id { get; set; }
        public decimal? Tax_Amount { get; set; }
        public string? Fbr { get; set; }
        public BudgetLimitTypeStatus? Budget_Limit_Type { get; set; }
        public int? Budget_Limit_Value { get; set; }
        public int Priority { get; set; }
        public DateTime Start_Date { get; set; }
        public DateTime End_Date { get; set; }
        public CampaginStatus Status { get; set; }
        [Required]
        public Guid Business_Id { get; set; }
        [Required]
        public Guid Business_Location_Id { get; set; }

        public List<CampaignBankFullCreateDto> Banks { get; set; } = [];
    }
}
