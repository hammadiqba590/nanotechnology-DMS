using NanoDMSAdminService.Blocks;

namespace NanoDMSAdminService.DTO.PosTerminalStatusHistory
{
    public class PosTerminalStatusHistoryCreateDto
    {
        public Guid Pos_Terminal_Id { get; set; }
        public TerminalHistoryStatus? Status { get; set; }
        public string? Notes { get; set; }
        public Guid Business_Id { get; set; }
        public Guid Business_Location_Id { get; set; }
    }
}
