namespace NanoDMSAdminService.DTO.CardBin
{
    public class CardBinBulkUploadResponse
    {
        public int TotalRows { get; set; }
        public int ImportedRows { get; set; }
        public int FailedRows { get; set; }
        public List<CardBinUploadResult> RowResults { get; set; } = new List<CardBinUploadResult>();
    }
}
