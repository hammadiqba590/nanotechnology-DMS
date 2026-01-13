namespace NanoDMSAdminService.DTO.PosTerminalAssignment
{
    public class PosTerminalAssignmentCreateDto
    {
        public Guid PosTerminal_Id { get; set; }
        public string Mid { get; set; } = "";
        public string Tid { get; set; } = "";
        public DateTime Assigned_At { get; set; }
        public DateTime? Unassigned_At { get; set; }
        public Guid Business_Id { get; set; }
        public Guid Business_Location_Id { get; set; }
    }

}
