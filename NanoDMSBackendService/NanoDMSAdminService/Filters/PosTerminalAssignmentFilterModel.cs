namespace NanoDMSAdminService.Filters
{
    public class PosTerminalAssignmentFilterModel
    {
        public Guid? PosTerminal_Id { get; set; }
        public string? Mid { get; set; }
        public string? Tid { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

}
