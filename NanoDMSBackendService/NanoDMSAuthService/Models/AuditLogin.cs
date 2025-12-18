namespace NanoDMSAuthService.Models
{
    public class AuditLogin
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string MacAddress { get; set; } = string.Empty;
        public string PcName { get; set; } = string.Empty;
        public DateTime LoginDateTime { get; set; }
    }
}
