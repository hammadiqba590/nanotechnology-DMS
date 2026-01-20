using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NanoDMSAuthService.Data;
using NanoDMSAuthService.Models;
using NanoDMSAuthService.Services;
using NanoDMSAuthService.Common;
using NanoDMSAuthService.DTO;
using System.Text.Json;
using NanoDMSSharedLibrary;

namespace NanoDMSAuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        #region ReadOnly

        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly EmailService _emailService;
        private readonly ApiServiceHelper _apiServiceHelper;

        #endregion

        #region Constructor
        public UserController(
           AppDbContext context,
           IConfiguration configuration,
           UserManager<AppUser> userManager,
           ApiServiceHelper apiServiceHelper,
           RoleManager<IdentityRole> roleManager, EmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
            _apiServiceHelper = apiServiceHelper;
            _emailService = emailService;
        }

        #endregion

        #region Apis

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserModel register)
        {
            // Check if the user already exists
            var userExists = await _userManager.FindByNameAsync(register.UserName);
            if (userExists != null)
                return Ok(new { Message = "User Already Exists." });

            var userExistsEmail = await _userManager.FindByEmailAsync(register.Email);
            if (userExistsEmail != null)
                return Ok(new { Message = "User Already Exists." });

            // Generate a random secure password
            var generatedPassword = GenerateSecurePassword();

            // Create the new user
            var user = new AppUser
            {
                UserName = register.UserName,
                Email = register.Email,
                EmailConfirmed = false, // Mark email as not confirmed
                NormalizedUserName = register.Email
            };

            // Create the user with the generated password
            var result = await _userManager.CreateAsync(user, generatedPassword);
            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    Message = "Failed To Create User.",
                    Errors = result.Errors.Select(e => e.Description)
                });
            }



            // Assign the user to the role if specified
            if (!string.IsNullOrEmpty(register.Role))
            {
                if (!await _roleManager.RoleExistsAsync(register.Role))
                {
                    var roleResult = await _roleManager.CreateAsync(new IdentityRole(register.Role));
                    if (!roleResult.Succeeded)
                        return BadRequest(new { Message = "Failed To Create Role.", Errors = roleResult.Errors.Select(e => e.Description) });
                }
                await _userManager.AddToRoleAsync(user, register.Role);
            }

            // Path to the HTML file
            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplate", "registerOtp.html");

            // Read the content of the HTML file
            string htmlTemplate = await System.IO.File.ReadAllTextAsync(templatePath);

            // Replace placeholders with actual values
            htmlTemplate = htmlTemplate
                .Replace("{{generatedUserName}}", register.UserName)
                .Replace("{{generatedPassword}}", generatedPassword);

            // Send the email
            await SendEmailAsync(register.Email, "New Registered Password", htmlTemplate);
            return Ok(new { Message = "User Registered Successfully.Username And Password Has Been Sent To Your Email", UserID = user.Id });
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost("create-role")]
        public async Task<IActionResult> CreateRole([FromBody] RoleUserModel model)
        {
            if (string.IsNullOrEmpty(model.RoleName))
            {
                return BadRequest("Role Name Cannot Be Empty.");
            }

            // Check if the role already exists
            var roleExist = await _roleManager.RoleExistsAsync(model.RoleName);
            if (roleExist)
            {
                return Ok(new { Message = $"Role '{model.RoleName}' Already Exists." });
            }

            // Create the new role
            var role = new IdentityRole(model.RoleName);
            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                return Ok(new { Message = $"Role '{model.RoleName}' Created Successfully." });
            }
            else
            {
                return StatusCode(500, new { Message = "Failed To Create The Role.", Errors = result.Errors });
            }
        }


        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-users")]
        public async Task<IActionResult> GetUsers()
        {
            // Retrieve all users from the UserManager
            var users = await _userManager.Users.ToListAsync();


            if (users.Any())
            {
                // Return the users in the response
                return Ok(users.Select(user => new
                {
                    user.Id,
                    user.UserName,
                    user.Email
                }));
            }
            else
            {
                return NotFound(new { Message = "No Users Found." });
            }
        }


        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpPost("get-user-by-id")]
        public async Task<IActionResult> GetUserById(UserByIdModel userById)
        {

            try
            {
                // Retrieve user by Id from the UserManager
                var user = await _userManager.FindByIdAsync(userById.UserId);

                if (user == null)
                {
                    return NotFound(new { Message = "User Not Found." });
                }

                // Return the user details in the response
                return Ok(new
                {
                    user.Id,
                    user.UserName,
                    user.Email
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-roles")]
        public async Task<IActionResult> GetRoles()
        {

            // Create the new role
            var result = await _roleManager.Roles.ToListAsync();

            if (result.Count > 0)
            {
                return Ok(result);
            }
            else
            {
                return StatusCode(500, new { Message = "Failed To Get The Role." });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            // Validate configuration values to ensure they are not null
            var jwtKey = _configuration["Jwt:Key"];
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var jwtAudience = _configuration["Jwt:Audience"];

            if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
            {
                return StatusCode(500, new { Message = "JWT Configuration Values Are Missing Or Invalid." });
            }

            // Find user by username
            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user == null || string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Email))
            {
                return BadRequest(new { Message = "Invalid Username Or Password." });
            }

            // Ensure user.UserName is not null before assignment
            var userName = user.UserName ?? string.Empty;

            // Find the user profile by UserId
            var userProfile = await _context.UserProfile.FirstOrDefaultAsync(up => up.UserId == Guid.Parse(user.Id));

            // Check if user profile exists and if it is deleted
            if (userProfile != null && userProfile.Deleted)
            {
                return BadRequest(new { Message = "User Is Inactive. Please Contact Admin For Further Details." });
            }

            // Fetch client details
            var ipAddress = ClientInfoHelper.GetIpAddress();
            var pcName = ClientInfoHelper.GetPcName();
            var macAddress = ClientInfoHelper.GetMacAddress();

            // Hash the password securely
            var hashedPassword = _userManager.PasswordHasher.HashPassword(user, model.Password);

            // Log attempt in the audit_login table
            var auditLogin = new AuditLogin
            {
                UserId = user.Id,
                UserName = userName, // Use the non-nullable userName variable
                Password = hashedPassword, // Store hashed password
                IpAddress = ipAddress,
                MacAddress = macAddress,
                PcName = pcName,
                LoginDateTime = DateTime.UtcNow
            };

            await _context.AuditLogins.AddAsync(auditLogin);
            await _context.SaveChangesAsync();

            // Check if the account is locked out
            if (await _userManager.IsLockedOutAsync(user))
            {
                // Refresh token for locked-out user
                var roles = await _userManager.GetRolesAsync(user);

                var jwt = new JwtHelper(jwtKey, jwtIssuer, jwtAudience);

                var (newJwtToken, expirytoken) = jwt.GenerateJwtToken(user.UserName!, user.Email!, roles);

                // Save the refreshed JWT token in the AspNetUserTokens table
                await _userManager.SetAuthenticationTokenAsync(user, "MyApp", "JwtToken", newJwtToken);

                // Calculate lockout remaining time
                var lockoutEnd = user.LockoutEnd ?? DateTime.UtcNow;
                var timeRemaining = lockoutEnd - DateTime.UtcNow;

                if (timeRemaining.TotalSeconds > 0)
                {
                    return BadRequest(new
                    {
                        Message = $"Your Account Is Locked. Please Try Again After {timeRemaining.Minutes} Minute(s).",
                        RefreshedToken = newJwtToken
                    });
                }
                else
                {
                    return BadRequest(new { Message = "Your Account Is Locked. Please Try Again Later.", RefreshedToken = newJwtToken });
                }
            }

            // Check if email is confirmed
            if (!user.EmailConfirmed)
            {
                // Generate JWT token
                var userRole = await _userManager.GetRolesAsync(user);

                var jwt = new JwtHelper(jwtKey, jwtIssuer, jwtAudience);

                var (firstTimeLoginToken, expirytoken) = jwt.GenerateJwtToken(user.UserName!, user.Email!, userRole);

                // In your method:
                var apiGatewayBaseUrl = _configuration["GlobalConfiguration:BaseUrl"];

               var firstTimeLoginLink = $"{apiGatewayBaseUrl}/apigateway/AuthService/Account/FirstTimeLogin?userId={user.Id}&token={firstTimeLoginToken}";
               // var firstTimeLoginLink = $"{apiGatewayBaseUrl}/apigateway/GroAuthService/Account/FirstTimeLogin?userId={user.Id}&token={firstTimeLoginToken}";

                // Send the confirmation email with the link
                await SendConfirmationEmail(user.Email, firstTimeLoginLink);

                return Ok(new { Message = "First-Time Login Email Has Been Sent To Your Email Address.", });
            }

            // Check 90-day password expiration
            var lastPasswordChange = await GetLastPasswordChangeDate(user.Id);

            if (lastPasswordChange != null && (DateTime.UtcNow - lastPasswordChange.Value).TotalDays > 90)
            {
                return BadRequest(new { Message = "Password Has Expired. Please Reset Your Password." });
            }

            // Check the password
            var result = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!result)
            {
                // Increment the failed access attempts
                await _userManager.AccessFailedAsync(user);

                // Retrieve the updated failed attempts count
                var failedAttempts = await _userManager.GetAccessFailedCountAsync(user);

                // Check if the failed attempts exceed the threshold
                if (failedAttempts >= 5)
                {
                    // Lock the user out after 5 failed attempts
                    return BadRequest(new { Message = "Account Locked Due To Too Many Failed Login Attempts. Try Again In 30 Minutes." });
                }

                return BadRequest(new { Message = "Invalid Credentials." });
            }

            // Reset the failed attempts counter if login is successful
            await _userManager.ResetAccessFailedCountAsync(user);

            // Generate JWT token
            var userRoles = await _userManager.GetRolesAsync(user);

            var jwtHelper = new JwtHelper(jwtKey, jwtIssuer, jwtAudience);

            var (jwtToken, expiry) = jwtHelper.GenerateJwtToken(user.UserName!, user.Email!, userRoles);

            // Convert UTC to Pakistan Standard Time
            var expiryLocal = TimeZoneInfo.ConvertTimeFromUtc(expiry, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time"));

            // Save the JWT token in the AspNetUserTokens table
            await _userManager.SetAuthenticationTokenAsync(user, "MyApp", "JwtToken", jwtToken);
            
            var BaseUrl = _configuration["GlobalConfiguration:BaseUrl"];

            // URLs should come from configuration ideally
            string businessUsersUrl = $"{BaseUrl}/apigateway/BusinessService/BusinessUser" + "/get-business-users";
            string locationUsersUrl = $"{BaseUrl}/apigateway/BusinessService/BusinessLocationUser" + "/get-business-location-users";
            

            var businessUsersJson = await _apiServiceHelper.SendRequestAsync<object>(
    businessUsersUrl, HttpMethod.Get, null, jwtToken);

            var locationUsersJson = await _apiServiceHelper.SendRequestAsync<object>(
                locationUsersUrl, HttpMethod.Get, null, jwtToken);

            

            var userGuid = Guid.Parse(user.Id);

            // Optional chaining on null and existence of properties
            var businessUser = businessUsersJson?.TryGetProperty("businessUser", out var businessUserArray) == true
                ? businessUserArray.EnumerateArray().FirstOrDefault(x =>
                    x.TryGetProperty("userId", out var userIdProp) && userIdProp.GetGuid() == userGuid)
                : default;

            var locationUsersArray = locationUsersJson?.TryGetProperty("businessLocationUser", out var locationArray) == true
                ? locationArray.EnumerateArray()
                    .Where(x => x.TryGetProperty("userId", out var userIdProp) && userIdProp.GetGuid() == userGuid)
                    .ToList()
                : new List<JsonElement>();

            

            var businessId = businessUser.ValueKind != JsonValueKind.Undefined
                ? businessUser.GetProperty("businessId").GetGuid()
                : (Guid?)null;

            var businessLocationIds = locationUsersArray
                .Select(x => x.GetProperty("businessLocationId").GetGuid())
                .ToList();

            


            return Ok(new
            {
                Token = jwtToken,
                Expiry = expiryLocal.ToString("yyyy-MM-dd HH:mm:ss"), // Simplified date and time format
                User = user.UserName,
                UserId = user.Id,
                Roles = userRoles,
                BusinessId = businessId,
                BusinessLocationId = businessLocationIds,
            });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest(new { Message = "User not found." });

            // Generate a 6-digit OTP
            var otp = new Random().Next(100000, 999999).ToString();

            // Save the OTP temporarily (e.g., in-memory, cache, or database)
            user.SecurityStamp = otp;
            await _userManager.UpdateAsync(user);

            // Path to the HTML file
            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplate", "otp.html");

            // Read the content of the HTML file
            string htmlTemplate = await System.IO.File.ReadAllTextAsync(templatePath);

            // Replace placeholder with actual OTP
            htmlTemplate = htmlTemplate.Replace("{{OTP}}", otp);


            // Send the OTP via email
            await SendEmailAsync(model.Email, "Forgot Password OTP", htmlTemplate);

            return Ok(new { Message = "OTP Sent To Your Email." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return BadRequest(new { Message = "User Not Found." });

            // Validate OTP
            if (user.SecurityStamp != model.Otp)
                return BadRequest(new { Message = "Invalid OTP." });

            // Validate password policy
            if (!IsValidPassword(model.NewPassword))
                return BadRequest(new { Message = "Password Does Not Meet The Policy Requirements." });

            // Check against password history
            var lastPasswords = _context.PasswordHistory
                .Where(p => p.UserId == user.Id)
                .OrderByDescending(p => p.CreatedAt)
                .Take(4)
                .Select(p => p.PasswordHash)
                .ToList();

            if (lastPasswords.Any(p => _userManager.PasswordHasher.VerifyHashedPassword(user, p, model.NewPassword) == PasswordVerificationResult.Success))
                return BadRequest(new { Message = "New Password Cannot Be One Of The Last 4 Passwords." });

            // Update the password
            var resetResult = await _userManager.ResetPasswordAsync(user, await _userManager.GeneratePasswordResetTokenAsync(user), model.NewPassword);
            if (!resetResult.Succeeded)
                return BadRequest(new { resetResult.Errors });

            // Save the password to history
            var passwordHistory = new PasswordHistory
            {
                UserId = user.Id,
                UserName = user.UserName?? string.Empty,
                PasswordHash = _userManager.PasswordHasher.HashPassword(user, model.NewPassword),
                CreatedAt = DateTime.UtcNow

            };
            _context.PasswordHistory.Add(passwordHistory);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Password Reset Successfully." });
        }

        [HttpPost("first-time-login")]
        public async Task<IActionResult> FirstTimeLogin([FromBody] FirstTimeLoginModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return Ok(new { Message = "User Not Found." });
            // Validate configuration values to ensure they are not null
            var jwtKey = _configuration["Jwt:Key"];
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var jwtAudience = _configuration["Jwt:Audience"];

            if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
            {
                return StatusCode(500, new { Message = "JWT Configuration Values Are Missing Or Invalid." });
            }

            var jwtHelper = new JwtHelper(jwtKey, jwtIssuer, jwtAudience);

            // Validate the token
            if (!jwtHelper.ValidateJwtToken(model.Token, out var principal))
                return Ok(new { Message = "Invalid Or Expired Token." });

            if (model.NewPassword != model.ConfirmPassword)
                return Ok(new { Message = "Passwords Do Not Match." });

            // Validate password against policy
            if (!ValidatePasswordPolicy(model.NewPassword))
                return Ok(new { Message = "Password Does Not Meet Complexity Requirements." });

            // Update password
            await _userManager.RemovePasswordAsync(user);
            var result = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (!result.Succeeded)
                return Ok(new { Message = "Failed To Set New Password.", Errors = result.Errors.Select(e => e.Description) });

            // Mark email as confirmed
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            // Save password to history
            await SavePasswordToHistory(user.Id!, user.UserName!, model.NewPassword);

            // Unlock the account if it's locked
            await _userManager.ResetAccessFailedCountAsync(user);

            return Ok(new { Message = "Success" });
        }

        #endregion

        #region Functions
        private async Task SendConfirmationEmail(string email, string firstTimeLoginLink)
        {

            var subject = "Confirm Your Email and Set a New Password";

            // Path to the HTML file
            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplate", "emailConfirm.html");

            // Read the content of the HTML file
            string htmlTemplate = await System.IO.File.ReadAllTextAsync(templatePath);

            // Replace placeholders with actual values
            htmlTemplate = htmlTemplate
                .Replace("{firstTimeLoginLink}", firstTimeLoginLink);

            await _emailService.SendEmailAsync(email, subject, htmlTemplate);
        }
        private string GenerateSecurePassword()
        {
            const int passwordLength = 12;

            // Define character sets for the password policy
            const string upperCaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowerCaseChars = "abcdefghijklmnopqrstuvwxyz";
            const string digitChars = "0123456789";
            const string specialChars = "!@#$%^&*()";
            const string allChars = upperCaseChars + lowerCaseChars + digitChars + specialChars;

            // Ensure at least one character from each set is included
            var random = new Random();
            var password = new List<char>
    {
        upperCaseChars[random.Next(upperCaseChars.Length)],
        lowerCaseChars[random.Next(lowerCaseChars.Length)],
        digitChars[random.Next(digitChars.Length)],
        specialChars[random.Next(specialChars.Length)]
    };

            // Fill the remaining characters randomly
            for (int i = password.Count; i < passwordLength; i++)
            {
                password.Add(allChars[random.Next(allChars.Length)]);
            }

            // Shuffle the password to avoid predictable patterns
            return new string(password.OrderBy(x => random.Next()).ToArray());
        }
        private async Task SendEmailAsync(string email, string subject, string message)
        {

            await _emailService.SendEmailAsync(email, subject, message);

        }
        //private async Task<string> GetMostRecentPassword(string userId)
        //{
        //    return await _context.PasswordHistory
        //        .Where(ph => ph.UserId == userId)
        //        .OrderByDescending(ph => ph.CreatedAt)
        //        .Select(ph => ph.PasswordHash)
        //        .FirstOrDefaultAsync();
        //}
        //private async Task<string> GetFirstTimePassword(string userName)
        //{
        //    var user = await _userManager.FindByNameAsync(userName);
        //    if (user == null)
        //        return null;

        //    return user.PasswordHash; // Retrieve the current password hash
        //}
        private async Task<DateTime?> GetLastPasswordChangeDate(string userId)
        {
            return await _context.PasswordHistory
                .Where(ph => ph.UserId == userId)
                .OrderByDescending(ph => ph.CreatedAt)
                .Select(ph => ph.CreatedAt)
                .FirstOrDefaultAsync();
        }
        private async Task SavePasswordToHistory(string userid, string username, string password)
        {
            var passwordHash = _userManager.PasswordHasher.HashPassword(new AppUser(), password);

            var passwordHistory = new PasswordHistory
            {
                UserId = userid,
                UserName = username,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow
            };

            _context.PasswordHistory.Add(passwordHistory);
            await _context.SaveChangesAsync();
        }
        private bool IsValidPassword(string password)
        {
            // Enforce password policy: 12-128 characters, capital, small, numeric, special character
            var hasUpper = password.Any(char.IsUpper);
            var hasLower = password.Any(char.IsLower);
            var hasDigit = password.Any(char.IsDigit);
            var hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));
            var isValidLength = password.Length >= 12 && password.Length <= 128;

            return hasUpper && hasLower && hasDigit && hasSpecial && isValidLength;
        }
        private bool ValidatePasswordPolicy(string password)
        {
            var hasUpper = password.Any(char.IsUpper);
            var hasLower = password.Any(char.IsLower);
            var hasDigit = password.Any(char.IsDigit);
            var hasSpecial = password.Any(ch => "!@#$%^&*()".Contains(ch));
            return hasUpper && hasLower && hasDigit && hasSpecial && password.Length >= 12;
        }
    //    private (string Token, DateTime Expiry) GenerateJwtToken(AppUser user, IList<string> roles)
    //    {
    //        var key = _configuration["Jwt:Key"];
    //        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    //        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    //        var claims = new List<Claim>
    //{
    //    new Claim(ClaimTypes.Name, user.UserName),
    //    new Claim(ClaimTypes.Email, user.Email)
    //};

    //        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

    //        var token = new JwtSecurityToken(
    //            issuer: _configuration["Jwt:Issuer"],
    //            audience: _configuration["Jwt:Audience"],
    //            claims: claims,
    //            expires: DateTime.Now.AddHours(1),
    //            signingCredentials: credentials);

    //        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

    //        return (jwtToken, token.ValidTo);
    //    }

    //    private bool ValidateJwtToken(string token, out ClaimsPrincipal principal)
    //    {
    //        principal = null;

    //        try
    //        {
    //            var key = _configuration["Jwt:Key"];
    //            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

    //            var tokenHandler = new JwtSecurityTokenHandler();

    //            // Token validation parameters
    //            var validationParameters = new TokenValidationParameters
    //            {
    //                ValidateIssuerSigningKey = true,
    //                IssuerSigningKey = securityKey,
    //                ValidateIssuer = true,
    //                ValidateAudience = true,
    //                ValidIssuer = _configuration["Jwt:Issuer"],
    //                ValidAudience = _configuration["Jwt:Audience"],
    //                ClockSkew = TimeSpan.Zero // Optional: Reduces allowed clock skew to 0 for stricter validation
    //            };

    //            // Validate the token and extract the principal
    //            principal = tokenHandler.ValidateToken(token, validationParameters, out _);
    //            return true;
    //        }
    //        catch (Exception ex)
    //        {
    //            // Log the exception (optional)
    //            Console.WriteLine($"Token validation failed: {ex.Message}");
    //            return false;
    //        }
    //    }
        #endregion


    }
}
