using NanoDMSAdminService.Blocks;
using NanoDMSAdminService.Common;

namespace NanoDMSAdminService.DTO.PosTerminalStatusHistory
{
    public class PosTerminalStatusHistoryDto : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid Pos_Terminal_Id { get; set; }
        public TerminalHistoryStatus? Status { get; set; }
        public string? Notes { get; set; }
    }

}
