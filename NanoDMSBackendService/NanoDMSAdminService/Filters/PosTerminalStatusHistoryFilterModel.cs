using NanoDMSAdminService.Blocks;

namespace NanoDMSAdminService.Filters
{
    public class PosTerminalStatusHistoryFilterModel
    {
        public Guid? Pos_Terminal_Id { get; set; }
        public TerminalHistoryStatus? Status { get; set; }
        public string? Notes { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
