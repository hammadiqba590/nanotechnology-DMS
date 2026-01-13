using NanoDMSAdminService.Common;

namespace NanoDMSAdminService.DTO.PosTerminalMaster
{
    public class PosTerminalMasterDto : BaseEntity
    {
        public Guid Id { get; set; }
        public string Serial_Number { get; set; } = "";
        public string? Terminal_Code { get; set; }
        public string? Company { get; set; }
        public string? Model_Number { get; set; }
        public string? Software_Version { get; set; }
        public string? Description { get; set; }

    }

}
