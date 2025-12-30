using NanoDMSAdminService.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NanoDMSAdminService.Models
{
    public class CardBrand : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; } = "";

        public ICollection<CardBin> Card_Bins { get; set; } = new List<CardBin>();
    }
}
