using NanoDMSAuthService.Models;

namespace NanoDMSAuthService.DTO
{
    public class UserProfileModel
    {
        public RegisterUserModel Register { get; set; } = new RegisterUserModel();
        public RegisterUserProfileModel Profile { get; set; } = new RegisterUserProfileModel();
    }

}
