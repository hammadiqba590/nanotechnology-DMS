using NanoDMSAdminService.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NanoDMSAdminService.DTO.PspDocument
{
    public class PspDocumentDto : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid Psp_Id { get; set; }
        public string? Psp_Name { get; set; }
        public string Doc_Type { get; set; } = null!;
        public string Doc_Url { get; set; } = null!;
    }
}
