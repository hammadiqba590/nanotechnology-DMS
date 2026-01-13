namespace NanoDMSAdminService.DTO.PspDocument
{
    public class PspDocumentCreateDto
    {
        public Guid Psp_Id { get; set; }
        public string Doc_Type { get; set; } = null!;
        public string Doc_Url { get; set; } = null!;
        public Guid Business_Id { get; set; }
        public Guid Business_Location_Id { get; set; }
    }
}
