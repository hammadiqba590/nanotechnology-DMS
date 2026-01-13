using System.ComponentModel.DataAnnotations;

namespace NanoDMSAdminService.DTO.PosTerminalMaster
{
    public class PosTerminalMasterCreateDto
    {
        [Required, MaxLength(100)]
        public string Serial_Number { get; set; } = "";
        [MaxLength(50)]
        public string? Terminal_Code { get; set; }
        [MaxLength(100)]
        public string? Company { get; set; }
        [MaxLength(100)]
        public string? Model_Number { get; set; }
        [MaxLength(50)]
        public string? Software_Version { get; set; }
        [MaxLength(255)]
        public string? Description { get; set; }
        public Guid Business_Id { get; set; }
        public Guid Business_Location_Id { get; set; }
    }

}
