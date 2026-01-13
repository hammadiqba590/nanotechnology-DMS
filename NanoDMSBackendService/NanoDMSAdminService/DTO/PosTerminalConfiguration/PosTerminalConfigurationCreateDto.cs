namespace NanoDMSAdminService.DTO.PosTerminalConfiguration
{
    public class PosTerminalConfigurationCreateDto
    {
        public Guid Pos_Terminal_Id { get; set; }
        public string Config_Key { get; set; } = "";
        public string Config_Value { get; set; } = "";
        public Guid Business_Id { get; set; }
        public Guid Business_Location_Id { get; set; }
    }

}
