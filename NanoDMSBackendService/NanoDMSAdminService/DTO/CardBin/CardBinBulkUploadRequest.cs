namespace NanoDMSAdminService.DTO.CardBin
{
    public class CardBinBulkUploadRequest
    {
        public List<CardBinCsvRowDto> Rows { get; set; } = new List<CardBinCsvRowDto>();
    }

}
