namespace NanoDMSAdminService.Filters
{
    public class PosTerminalMasterFilterModel
    {
        public string Serial_Number { get; set; } = "";
        public string? Terminal_Code { get; set; }
        public string? Company { get; set; }
        public string? Model_Number { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

}
