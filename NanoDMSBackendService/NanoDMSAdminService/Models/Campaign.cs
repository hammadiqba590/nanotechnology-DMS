using NanoDMSAdminService.Blocks;
using NanoDMSAdminService.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NanoDMSAdminService.Models
{
    public class Campaign : BaseEntity
    {
        public Guid Id { get; set; }

        [Required]
        public string Campaign_Name { get; set; } = "";

        public string? Description { get; set; }

        [StringLength(3)]
        public string Currency { get; set; } = "PKR";
        public decimal? Tax_Amount { get; set; }
        public string? Fbr { get; set; }
        public CampaginStatus? Status { get; set; } //active, inactive,expired 
        public BudgetLimitTypeStatus? Budget_Limit_Type { get; set; } // 'hourly','daily','weekly','monthly','yearly'
        public int? Budget_Limit_Value { get; set; } //'Optional: max transactions or uses in the period',
        public int Priority { get; set; }
        public ICollection<CampaignBank> CampaignBanks { get; set; } = new List<CampaignBank>();

    }

}
