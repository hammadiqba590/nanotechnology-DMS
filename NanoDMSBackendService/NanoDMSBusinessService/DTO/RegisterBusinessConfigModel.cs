namespace NanoDMSBusinessService.DTO
{
    public class RegisterBusinessConfigModel
    {
        public string NameKey { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ConfigValue { get; set; } = string.Empty;
        public string ConfigType { get; set; } = string.Empty;
        public Guid BusinessId { get; set; }
        public Guid BusinessLocationId { get; set; }
    }
}
