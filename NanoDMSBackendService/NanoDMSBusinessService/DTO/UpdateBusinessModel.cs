namespace NanoDMSBusinessService.DTO
{
    public class UpdateBusinessModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public IFormFile? Logo { get; set; }
        public string Ntn { get; set; } = string.Empty;
        public string Stn { get; set; } = string.Empty;
        public string Tax3 { get; set; } = string.Empty;
        public string Tax4 { get; set; } = string.Empty;
    }
}
