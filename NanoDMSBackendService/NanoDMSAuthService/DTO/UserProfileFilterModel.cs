namespace NanoDMSAuthService.DTO
{
    public class UserProfileFilterModel
    {
        public string? UserName { get; set; } = null;
        public string? Email { get; set; }  = null ;
        public string? PhoneNumber { get; set; } = null;
        public string? UserRole { get; set; } = null;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
