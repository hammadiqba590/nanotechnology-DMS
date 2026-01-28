namespace NanoDMSAdminService.DTO.CardBin
{
    public class CardBinUploadResult
    {
        public string Bin { get; set; } = "";
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
