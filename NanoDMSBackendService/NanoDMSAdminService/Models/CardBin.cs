using NanoDMSAdminService.Blocks;
using NanoDMSAdminService.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NanoDMSAdminService.Models
{
    public class CardBin : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        public Guid Bank_Id { get; set; }
        [ForeignKey(nameof(Bank_Id))]
        public Bank? Bank { get; set; }

        [Required, MaxLength(12)]
        public string Card_Bin_Value { get; set; } = "";

        public Guid Card_Brand_Id { get; set; }
        [ForeignKey(nameof(Card_Brand_Id))]
        public CardBrand? Card_Brand { get; set; }

        public Guid Card_Type_Id { get; set; }
        [ForeignKey(nameof(Card_Type_Id))]
        public CardType? Card_Type { get; set; }

        public Guid? Card_Level_Id { get; set; }
        [ForeignKey(nameof(Card_Level_Id))]
        public CardLevel? Card_Level { get; set; }
        public LocalInternationalStatus? Local_International { get; set; } // local ,international
        public Guid? Country_Id { get; set; }
        [ForeignKey(nameof(Country_Id))]
        public Country? Country { get; set; }

        public ICollection<CampaignCardBin> Campaign_Card_Bins { get; set; } = new List<CampaignCardBin>();

    }

}
