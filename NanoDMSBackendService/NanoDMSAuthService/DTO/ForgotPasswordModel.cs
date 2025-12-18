using System.ComponentModel.DataAnnotations;

namespace NanoDMSAuthService.DTO
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
