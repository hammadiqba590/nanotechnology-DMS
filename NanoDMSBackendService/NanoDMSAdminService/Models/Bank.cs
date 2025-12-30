using NanoDMSAdminService.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NanoDMSAdminService.Models
{
    public class Bank : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = "";

        [Required, MaxLength(50)]
        public string Short_Name { get; set; } = "";

        [Required, MaxLength(20)]
        public string Short_Code { get; set; } = "";

        [MaxLength(20)]
        public string? Swift_Code { get; set; }
        public Guid Country_Id { get; set; }

        [ForeignKey(nameof(Country_Id))]
        public Country? Country { get; set; }

        public ICollection<CampaignBank> Campaign_Banks { get; set; } = new List<CampaignBank>();
        public ICollection<CardBin> Card_Bins { get; set; } = new List<CardBin>();
    }

}
