using NanoDMSAdminService.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NanoDMSAdminService.Models
{
    public class PosTerminalMaster : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(100)]
        public string Serial_Number { get; set; } = "";

        [MaxLength(50)]
        public string? Terminal_Code { get; set; }

        [MaxLength(100)]
        public string? Company { get; set; }

        [MaxLength(100)]
        public string? Model_Number { get; set; }

        [MaxLength(255)]
        public string? Description { get; set; }

        [MaxLength(50)]
        public string? Software_Version { get; set; }

        public ICollection<PosTerminalAssignment>? PosTerminal_Assignments { get; set; } = new List<PosTerminalAssignment>();

        public ICollection<PosTerminalConfiguration>? PosTerminal_Configurations { get; set; } = new List<PosTerminalConfiguration>();

        public ICollection<PosTerminalStatusHistory>? PosTerminal_Status_Histories { get; set; } = new List<PosTerminalStatusHistory>();
    }

}
