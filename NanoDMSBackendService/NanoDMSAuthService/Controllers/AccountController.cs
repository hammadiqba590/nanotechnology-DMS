using Microsoft.AspNetCore.Mvc;
using NanoDMSAuthService.DTO;

namespace NanoDMSAuthService.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet("Account/FirstTimeLogin")]
        public IActionResult FirstTimeLogin(string userId, string token)
        {
            // Create the FirstTimeLoginModel and assign values from query string
            var firstTimeLogin = new FirstTimeLoginModel
            {
                UserId = userId,
                Token = token
            };

            // Return the view with the model data
            return View(firstTimeLogin);
        }


    }

}
