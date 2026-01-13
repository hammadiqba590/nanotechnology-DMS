namespace NanoDMSAdminService.Filters
{
    public class PosTerminalConfigurationFilterModel
    {
        public Guid? Pos_Terminal_Id { get; set; }
        public string? Config_Key { get; set; } 
        public string? Config_Value { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
