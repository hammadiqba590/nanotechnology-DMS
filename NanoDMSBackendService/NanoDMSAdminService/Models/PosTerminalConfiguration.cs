using NanoDMSAdminService.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NanoDMSAdminService.Models
{
    public class PosTerminalConfiguration : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid Terminal_Id { get; set; }
        [ForeignKey(nameof(Terminal_Id))]
        public PosTerminalMaster? Pos_Terminal { get; set; }

        [Required, MaxLength(100)]
        public string Config_Key { get; set; } = "";

        [Required]
        public string Config_Value { get; set; } = ""; // JSON as string

        
    }

}
