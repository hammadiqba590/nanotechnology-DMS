using NanoDMSAdminService.Blocks;
using NanoDMSAdminService.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NanoDMSAdminService.Models
{
    public class PosTerminalStatusHistory : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid Pos_Terminal_Id { get; set; }
        [ForeignKey(nameof(Pos_Terminal_Id))]
        public PosTerminalMaster? Pos_Terminal { get; set; }
        public TerminalHistoryStatus? Status { get; set; }  // active, inactive, maintenance,decommissioned
        public string? Notes { get; set; }
    }

}
