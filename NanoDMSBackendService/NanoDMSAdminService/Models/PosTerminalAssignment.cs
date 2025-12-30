using NanoDMSAdminService.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NanoDMSAdminService.Models
{
    public class PosTerminalAssignment : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid PosTerminal_Id { get; set; }
        [ForeignKey(nameof(PosTerminal_Id))]
        public PosTerminalMaster? Pos_Terminal { get; set; }

        [Required, MaxLength(50)]
        public string Mid { get; set; } = "";

        [Required, MaxLength(50)]
        public string Tid { get; set; } = "";
        public DateTime Assigned_At { get; set; }
        public DateTime? Unassigned_At { get; set; }

    }

}
