using NanoDMSAdminService.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NanoDMSAdminService.Filters
{
    public class PspDocumentFilterModel
    {
        public Guid? Psp_Id { get; set; }
        public string Doc_Type { get; set; } = null!;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
