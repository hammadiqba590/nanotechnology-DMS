using NanoDMSAdminService.Common;

namespace NanoDMSAdminService.DTO.PosTerminalAssignment
{
    public class PosTerminalAssignmentDto : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid PosTerminal_Id { get; set; }
        public string Mid { get; set; } = "";
        public string Tid { get; set; } = "";
        public DateTime Assigned_At { get; set; }
        public DateTime? Unassigned_At { get; set; }
    }

}
