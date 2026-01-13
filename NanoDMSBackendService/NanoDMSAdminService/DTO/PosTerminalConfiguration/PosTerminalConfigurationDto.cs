using NanoDMSAdminService.Common;

namespace NanoDMSAdminService.DTO.PosTerminalConfiguration
{
    public class PosTerminalConfigurationDto : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid Pos_Terminal_Id { get; set; }
        public string Config_Key { get; set; } = "";
        public string Config_Value { get; set; } = "";
    }

}
