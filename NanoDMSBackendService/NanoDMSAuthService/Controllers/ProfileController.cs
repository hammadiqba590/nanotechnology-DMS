using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NanoDMSAuthService.Data;
using NanoDMSAuthService.DTO;
using NanoDMSAuthService.Models;
using NanoDMSAuthService.Services;

namespace NanoDMSAuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly EmailService _emailService;
        public ProfileController(AppDbContext context,
            IConfiguration configuration,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager, EmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
        }

        [Authorize]
        [HttpPost("register-profile")]
        public async Task<IActionResult> Register([FromForm] UserProfileModel model)
        {
            if (model == null || model.Register == null || model.Profile == null)
            {
                return BadRequest("Invalid request data.");
            }

            try
            {
                // Check if the user already exists
                var userExists = await _userManager.FindByNameAsync(model.Register.UserName);
                if (userExists != null)
                    return Ok(new { Message = "User already exists." });

                var userExistsEmail = await _userManager.FindByEmailAsync(model.Register.Email);
                if (userExistsEmail != null)
                    return Ok(new { Message = "User already exists." });

                // Generate a random secure password
                var generatedPassword = GenerateSecurePassword();

                // Create the new user
                var user = new AppUser
                {
                    UserName = model.Register.UserName,
                    Email = model.Register.Email,
                    PhoneNumber = model.Profile.MobileNumber,
                    EmailConfirmed = false, // Mark email as not confirmed
                    NormalizedUserName = model.Register.Email
                };

                // Create the user with the generated password
                var result = await _userManager.CreateAsync(user, generatedPassword);
                if (!result.Succeeded)
                {
                    return Ok(new
                    {
                        Message = "Failed to create user.",
                        Errors = result.Errors.Select(e => e.Description)
                    });
                }

                // Assign the user to the role if specified
                if (!string.IsNullOrEmpty(model.Register.Role))
                {
                    if (!await _roleManager.RoleExistsAsync(model.Register.Role))
                    {
                        var roleResult = await _roleManager.CreateAsync(new IdentityRole(model.Register.Role));
                        if (!roleResult.Succeeded)
                            return Ok(new { Message = "Failed to create role.", Errors = roleResult.Errors.Select(e => e.Description) });
                    }
                    await _userManager.AddToRoleAsync(user, model.Register.Role);
                }

                // Path to the HTML file
                string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplate", "registerOtp.html");

                // Read the content of the HTML file
                string htmlTemplate = await System.IO.File.ReadAllTextAsync(templatePath);

                // Replace placeholders with actual values
                htmlTemplate = htmlTemplate
                    .Replace("{{generatedUserName}}", model.Register.UserName)
                    .Replace("{{generatedPassword}}", generatedPassword);

                // Send the email
                await SendEmailAsync(model.Register.Email, "New Registered Password", htmlTemplate);

                // Check if User.Identity is null
                if (User?.Identity?.Name == null)
                    return Unauthorized(new { Message = "User identity is not available." });

                // Check if user exists
                var userName = User.Identity.Name;
                if (string.IsNullOrEmpty(userName))
                    return Unauthorized(new { Message = "User name is not available." });

                var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
                if (superuser == null)
                {
                    return Unauthorized("User not found.");
                }
                var userId = superuser.Id; // Use this as your userId

                // Step 4: Save user profile
                if (string.IsNullOrEmpty(model.Profile.FirstName))
                {
                    return BadRequest(new { Message = "First Name is required." });
                }

                if (string.IsNullOrEmpty(model.Profile.LastName))
                {
                    return BadRequest(new { Message = "Last Name is required." });
                }

                if (string.IsNullOrEmpty(model.Profile.MobileNumber) || !Regex.IsMatch(model.Profile.MobileNumber, @"^\d{11}$"))
                {
                    return BadRequest(new { Message = "Mobile Number is required and should be a 11-digit number." });
                }

                UserProfile userProfile = new UserProfile
                {
                    Prefix = model.Profile.Prefix,
                    FirstName = model.Profile.FirstName,
                    LastName = model.Profile.LastName,
                    MobileNumber = model.Profile.MobileNumber,
                    PermanentAddress = model.Profile.PermanentAddress,
                    CurrentAddress = model.Profile.CurrentAddress,
                    City = model.Profile.City,
                    State = model.Profile.State,
                    Country = model.Profile.Country,
                    PostalCode = model.Profile.PostalCode,
                    PersonalEmailAddress = model.Profile.PersonalEmailAddress,
                    CNIC = model.Profile.CNIC,
                    Dob = model.Profile.Dob,
                    Gender = model.Profile.Gender,
                    MaritalStatus = model.Profile.MaritalStatus,
                    BloodGroup = model.Profile.BloodGroup,
                    AlternateNumber = model.Profile.AlternateNumber,
                    EmergencyContact = model.Profile.EmergencyContact,
                    NTNNumber = model.Profile.NTNNumber,
                    BankName = model.Profile.BankName,
                    BankBranch = model.Profile.BankBranch,
                    BankIBAN = model.Profile.BankIBAN,
                    BankAccountNumber = model.Profile.BankAccountNumber,
                    BankAccountName = model.Profile.BankAccountName,
                    CardNumber = model.Profile.CardNumber,
                    UserId = Guid.Parse(user.Id), // Assign UserId from created user
                    Id = Guid.NewGuid(),
                    SmsAlert = true,
                    EmailAlert = true,
                    CreateDate = DateTime.UtcNow,
                    Published = true,
                    CreateUser = Guid.Parse(user.Id) // Assuming the created user is also the creator
                };

                // Save profile in database
                _context.UserProfile.Add(userProfile);
                await _context.SaveChangesAsync();

                // Process and save the profile image
                if (model.Profile.Image != null && model.Profile.Image.Length > 0)
                {
                    // Define the root directory for images
                    var imagesRootPath = @"C:\Repos\dot net\PosApi\Images";

                    // Define subfolder dynamically (e.g., UserProfile, ProductImages, etc.)
                    var subFolder = "UserProfile"; // Change this dynamically based on use case
                    var staticPath = Path.Combine(imagesRootPath, subFolder);

                    if (!Directory.Exists(staticPath))
                    {
                        Directory.CreateDirectory(staticPath);
                    }

                    // Rename the image file using UserId
                    var fileExtension = Path.GetExtension(model.Profile.Image.FileName);
                    var newFileName = $"{userProfile.UserId}{fileExtension}";
                    var filePath = Path.Combine(staticPath, newFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Profile.Image.CopyToAsync(stream);
                    }

                    // Save relative path instead of absolute path
                    userProfile.Image = $"/Images/{subFolder}/{newFileName}";

                    _context.UserProfile.Update(userProfile);
                    await _context.SaveChangesAsync();
                }

                // Return success response
                return Ok(new { Message = "User profile registered successfully", UserProfile = userProfile });

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-profile-list")]
        public async Task<IActionResult> GetUserProfileList([FromQuery] UserProfileFilterModel filter)
        {
            try
            {
                var query = (from profile in _context.UserProfile
                             join user in _context.Users
                             on profile.UserId.ToString() equals user.Id
                             join userRole in _context.UserRoles
                             on user.Id equals userRole.UserId
                             join role in _context.Roles
                             on userRole.RoleId equals role.Id
                             select new UserProfileListModel
                             {
                                 Id = profile.Id,
                                 UserId = profile.UserId,
                                 Prefix = profile.Prefix,
                                 FirstName = profile.FirstName,
                                 LastName = profile.LastName,
                                 MobileNumber = profile.MobileNumber,
                                 Image = profile.Image,
                                 PermanentAddress = profile.PermanentAddress,
                                 CurrentAddress = profile.CurrentAddress,
                                 City = profile.City,
                                 State = profile.State,
                                 Country = profile.Country,
                                 PostalCode = profile.PostalCode,
                                 PersonalEmailAddress = profile.PersonalEmailAddress,
                                 CNIC = profile.CNIC,
                                 Dob = profile.Dob,
                                 Gender = profile.Gender,
                                 MaritalStatus = profile.MaritalStatus,
                                 BloodGroup = profile.BloodGroup,
                                 AlternateNumber = profile.AlternateNumber,
                                 EmergencyContact = profile.EmergencyContact,
                                 NTNNumber = profile.NTNNumber,
                                 BankName = profile.BankName,
                                 BankBranch = profile.BankBranch,
                                 BankIBAN = profile.BankIBAN,
                                 BankAccountNumber = profile.BankAccountNumber,
                                 BankAccountName = profile.BankAccountName,
                                 CardNumber = profile.CardNumber,
                                 CreateDate = profile.CreateDate,
                                 CreateUser = profile.CreateUser,
                                 LastUpdateDate = profile.LastUpdateDate,
                                 LastUpdateUser = profile.LastUpdateUser,
                                 Published = profile.Published,
                                 Deleted = profile.Deleted,
                                 // From AspNetUsers
                                 UserName = user.UserName ?? string.Empty,
                                 Email = user.Email ?? string.Empty,
                                 PhoneNumber = user.PhoneNumber ?? string.Empty,
                                 // From AspNetRoles
                                 UserRole = role.Name ?? string.Empty
                             }).AsQueryable();

                // Filters
                if (!string.IsNullOrEmpty(filter.UserName))
                    query = query.Where(q => q.UserName.Contains(filter.UserName));

                if (!string.IsNullOrEmpty(filter.Email))
                    query = query.Where(q => q.Email.Contains(filter.Email));

                if (!string.IsNullOrEmpty(filter.PhoneNumber))
                    query = query.Where(q => q.PhoneNumber.Contains(filter.PhoneNumber));

                if (!string.IsNullOrEmpty(filter.UserRole))
                    query = query.Where(q => q.UserRole.Contains(filter.UserRole)); 

                // Sort by CreateDate descending to get the latest profiles first
                query = query.OrderByDescending(q => q.CreateDate);

                // Pagination
                var totalRecords = await query.CountAsync();
                var userProfiles = await query
                    .Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToListAsync();

                if (!userProfiles.Any())
                {
                    return NoContent();
                }

                var response = new PaginatedResponseDto<UserProfileListModel>
                {
                    TotalRecords = totalRecords,
                    PageNumber = filter.PageNumber,
                    PageSize = filter.PageSize,
                    TotalPages = (int)Math.Ceiling((double)totalRecords / filter.PageSize),
                    Data = userProfiles
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            try
            {
                var userProfiles = await (from profile in _context.UserProfile
                                          join user in _context.Users
                                          on profile.UserId.ToString() equals user.Id
                                          join userRole in _context.UserRoles
                                          on user.Id equals userRole.UserId
                                          join role in _context.Roles
                                          on userRole.RoleId equals role.Id
                                          select new
                                          {
                                              // From UserProfile table
                                              profile.Id,
                                              profile.UserId,
                                              profile.Prefix,
                                              profile.FirstName,
                                              profile.LastName,
                                              profile.MobileNumber,
                                              profile.Image,
                                              profile.PermanentAddress,
                                              profile.CurrentAddress,
                                              profile.City,
                                              profile.State,
                                              profile.Country,
                                              profile.PostalCode,
                                              profile.PersonalEmailAddress,
                                              profile.CNIC,
                                              profile.Dob,
                                              profile.Gender,
                                              profile.MaritalStatus,
                                              profile.BloodGroup,
                                              profile.AlternateNumber,
                                              profile.EmergencyContact,
                                              profile.NTNNumber,
                                              profile.BankName,
                                              profile.BankBranch,
                                              profile.BankIBAN,
                                              profile.BankAccountNumber,
                                              profile.BankAccountName,
                                              profile.CardNumber,
                                              profile.Deleted,
                                              profile.Published,
                                              profile.CreateDate,
                                              profile.CreateUser,
                                              profile.LastUpdateDate,
                                              profile.LastUpdateUser,

                                              // From AspNetUsers table
                                              user.UserName,
                                              user.Email,
                                              user.PhoneNumber,

                                              // From AspNetRoles table
                                              UserRole = role.Name
                                          }).OrderByDescending(p => p.CreateDate).ToListAsync();


                if (userProfiles == null || !userProfiles.Any())
                {
                    return NoContent();
                }

                return Ok(userProfiles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-profile-by-mobile-or-card-or-username")]
        public async Task<IActionResult> GetUserProfileByMobileOrCardOrUserName([FromQuery] string? mobileNumber, [FromQuery] string? cardNumber, [FromQuery] string? userName)
        {
            try
            {
                if (string.IsNullOrEmpty(mobileNumber) && string.IsNullOrEmpty(cardNumber) && string.IsNullOrEmpty(userName))
                {
                    return BadRequest("At least one of mobile number or card number or user name  must be provided.");
                }

                var userProfiles = await (from profile in _context.UserProfile
                                          join user in _context.Users on profile.UserId.ToString() equals user.Id
                                          join userRole in _context.UserRoles on user.Id equals userRole.UserId
                                          join role in _context.Roles on userRole.RoleId equals role.Id
                                          where
                                              (!string.IsNullOrEmpty(mobileNumber) && profile.MobileNumber == mobileNumber)
                                              || (!string.IsNullOrEmpty(cardNumber) && profile.CardNumber == cardNumber)
                                              || (!string.IsNullOrEmpty(userName)&& user.UserName == userName)
                                          select new
                                          {
                                              // From UserProfile table
                                              profile.Id,
                                              profile.UserId,
                                              profile.Prefix,
                                              profile.FirstName,
                                              profile.LastName,
                                              profile.MobileNumber,
                                              profile.Image,
                                              profile.PermanentAddress,
                                              profile.CurrentAddress,
                                              profile.City,
                                              profile.State,
                                              profile.Country,
                                              profile.PostalCode,
                                              profile.PersonalEmailAddress,
                                              profile.CNIC,
                                              profile.Dob,
                                              profile.Gender,
                                              profile.MaritalStatus,
                                              profile.BloodGroup,
                                              profile.AlternateNumber,
                                              profile.EmergencyContact,
                                              profile.NTNNumber,
                                              profile.BankName,
                                              profile.BankBranch,
                                              profile.BankIBAN,
                                              profile.BankAccountNumber,
                                              profile.BankAccountName,
                                              profile.CardNumber,
                                              profile.Deleted,
                                              profile.Published,
                                              profile.CreateDate,
                                              profile.CreateUser,
                                              profile.LastUpdateDate,
                                              profile.LastUpdateUser,

                                              // From AspNetUsers table
                                              user.UserName,
                                              user.Email,
                                              user.PhoneNumber,

                                              // From AspNetRoles table
                                              UserRole = role.Name
                                          }).OrderByDescending(p => p.CreateDate).ToListAsync();

                if (userProfiles == null || !userProfiles.Any())
                {
                    return NotFound("No matching profiles found.");
                }

                return Ok(userProfiles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("get-profile-by-id")]
        public async Task<IActionResult> GetUserProfileById(UserProfileByIdModel userProfileById)
        {
            try
            {
                // Join UserProfile with AspNetUsers based on UserId
                var userProfile = await (from profile in _context.UserProfile
                                         join user in _context.Users
                                         on profile.UserId.ToString() equals user.Id
                                         where profile.Id == userProfileById.UserId
                                         select new
                                         {
                                             // From UserProfile table
                                             profile.Id,
                                             profile.UserId,
                                             profile.Prefix,
                                             profile.FirstName,
                                             profile.LastName,
                                             profile.MobileNumber,
                                             profile.Image,
                                             profile.PermanentAddress,
                                             profile.CurrentAddress,
                                             profile.City,
                                             profile.State,
                                             profile.Country,
                                             profile.PostalCode,
                                             profile.PersonalEmailAddress,
                                             profile.CNIC,
                                             profile.Dob,
                                             profile.Gender,
                                             profile.MaritalStatus,
                                             profile.BloodGroup,
                                             profile.AlternateNumber,
                                             profile.EmergencyContact,
                                             profile.NTNNumber,
                                             profile.BankName,
                                             profile.BankBranch,
                                             profile.BankIBAN,
                                             profile.BankAccountNumber,
                                             profile.BankAccountName,
                                             profile.CardNumber,
                                             profile.Deleted,
                                             profile.Published,
                                             profile.CreateDate,
                                             profile.CreateUser,
                                             profile.LastUpdateDate,
                                             profile.LastUpdateUser,
                                             // From AspNetUsers table
                                             user.UserName,
                                             user.Email,
                                             user.PhoneNumber
                                         }).FirstOrDefaultAsync();

                if (userProfile == null)
                {
                    return NotFound("Profile not found.");
                }

                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("get-profile-by-user-id")]
        public async Task<IActionResult> GetUserProfileByUserId(UserProfileByIdModel userProfileById)
        {
            try
            {
                // Join UserProfile with AspNetUsers based on UserId
                var userProfile = await (from profile in _context.UserProfile
                                         join user in _context.Users
                                         on profile.UserId.ToString() equals user.Id
                                         where profile.UserId == userProfileById.UserId
                                         select new
                                         {
                                             // From UserProfile table
                                             profile.Id,
                                             profile.UserId,
                                             profile.Prefix,
                                             profile.FirstName,
                                             profile.LastName,
                                             profile.MobileNumber,
                                             profile.Image,
                                             profile.PermanentAddress,
                                             profile.CurrentAddress,
                                             profile.City,
                                             profile.State,
                                             profile.Country,
                                             profile.PostalCode,
                                             profile.PersonalEmailAddress,
                                             profile.CNIC,
                                             profile.Dob,
                                             profile.Gender,
                                             profile.MaritalStatus,
                                             profile.BloodGroup,
                                             profile.AlternateNumber,
                                             profile.EmergencyContact,
                                             profile.NTNNumber,
                                             profile.BankName,
                                             profile.BankBranch,
                                             profile.BankIBAN,
                                             profile.BankAccountNumber,
                                             profile.BankAccountName,
                                             profile.CardNumber,
                                             profile.Deleted,
                                             profile.Published,
                                             profile.CreateDate,
                                             profile.CreateUser,
                                             profile.LastUpdateDate,
                                             profile.LastUpdateUser,
                                             // From AspNetUsers table
                                             user.UserName,
                                             user.Email,
                                             user.PhoneNumber
                                         }).FirstOrDefaultAsync();

                if (userProfile == null)
                {
                    return NotFound("Profile not found.");
                }

                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPut("edit-profile")]
        public async Task<IActionResult> EditUserProfile([FromForm] UpdateUserProfileModel updateDto)
        {
            // Validate the incoming data
            if (updateDto == null)
            {
                return BadRequest("Invalid data.");
            }

            if (string.IsNullOrEmpty(updateDto.FirstName) || string.IsNullOrEmpty(updateDto.LastName) || string.IsNullOrEmpty(updateDto.MobileNumber))
            {
                return BadRequest("First Name, Last Name, and Mobile Number are required.");
            }

            // Find the existing user profile by UserId
            var userProfile = await _context.UserProfile.FirstOrDefaultAsync(up => up.Id == Guid.Parse(updateDto.UserId.ToString()));
            if (userProfile == null)
            {
                return NotFound("UserProfile not found.");
            }
            // Check if User.Identity is null
            if (User?.Identity?.Name == null)
                return Unauthorized(new { Message = "User identity is not available." });

            // Check if user exists
            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(new { Message = "User name is not available." });

            var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (superuser == null)
            {
                return Unauthorized("User not found.");
            }

            var userId = superuser.Id; // Use this as your userId

            // Handle image update if a new image is provided
            if (updateDto.Image != null && updateDto.Image.Length > 0)
            {
                //// Define a static path (e.g., "C:/Images/ProfilePictures")
                //var staticPath = @"C:\Repos\dot net\PosApi\Images\UserProfile";

                // Define the root directory for images
                var imagesRootPath = @"C:\Repos\dot net\PosApi\Images";

                // Define subfolder dynamically (e.g., UserProfile, ProductImages, etc.)
                var subFolder = "UserProfile"; // Change this dynamically based on use case
                var staticPath = Path.Combine(imagesRootPath, subFolder);

                if (!Directory.Exists(staticPath))
                {
                    Directory.CreateDirectory(staticPath);
                }

                // Rename the image file using the UserId
                var fileExtension = Path.GetExtension(updateDto.Image.FileName);
                var newFileName = $"{userProfile.UserId}{fileExtension}";
                var filePath = Path.Combine(staticPath, newFileName);

                // Delete the old image if it exists
                if (!string.IsNullOrEmpty(userProfile.Image))
                {
                    var oldImagePath = userProfile.Image;

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await updateDto.Image.CopyToAsync(stream);
                }

                // Save the static path in the database (use relative or absolute path based on requirement)
                userProfile.Image = $"/Images/{subFolder}/{newFileName}"; // Save absolute file path

                
            }

            // Update other user profile properties
            userProfile.Prefix = updateDto.Prefix;
            userProfile.FirstName = updateDto.FirstName;
            userProfile.LastName = updateDto.LastName;
            userProfile.MobileNumber = updateDto.MobileNumber;
            userProfile.PermanentAddress = updateDto.PermanentAddress;
            userProfile.CurrentAddress = updateDto.CurrentAddress;
            userProfile.City = updateDto.City;
            userProfile.State = updateDto.State;
            userProfile.Country = updateDto.Country;
            userProfile.PostalCode = updateDto.PostalCode;
            userProfile.PersonalEmailAddress = updateDto.PersonalEmailAddress;
            userProfile.CNIC = updateDto.CNIC;
            userProfile.Dob = updateDto.Dob;
            userProfile.Gender = updateDto.Gender;
            userProfile.MaritalStatus = updateDto.MaritalStatus;
            userProfile.BloodGroup = updateDto.BloodGroup;
            userProfile.AlternateNumber = updateDto.AlternateNumber;
            userProfile.EmergencyContact = updateDto.EmergencyContact;
            userProfile.NTNNumber = updateDto.NTNNumber;
            userProfile.BankName = updateDto.BankName;
            userProfile.BankBranch = updateDto.BankBranch;
            userProfile.BankIBAN = updateDto.BankIBAN;
            userProfile.BankAccountNumber = updateDto.BankAccountNumber;
            userProfile.BankAccountName = updateDto.BankAccountName;
            userProfile.CardNumber = updateDto.CardNumber;
            userProfile.SmsAlert = updateDto.SmsAlert;
            userProfile.EmailAlert = updateDto.EmailAlert;

            // If validation passes, proceed to save the profile
            userProfile.LastUpdateDate = DateTime.UtcNow;
            userProfile.Published = true;
            userProfile.LastUpdateUser = Guid.Parse(userId); // Assign UserId who created the profile

            // Save the changes to the database
            try
            {
                _context.UserProfile.Update(userProfile);
                await _context.SaveChangesAsync();
                return Ok(new { Message = "User profile updated successfully", UserProfile = userProfile }); // Return updated profile
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [HttpDelete("delete-profile")]
        public async Task<IActionResult> DeleteUserProfile(DelteUserProfileModel deleteUser)
        {
            // Find the user profile by UserId
            var userProfile = await _context.UserProfile.FirstOrDefaultAsync(up => up.UserId == deleteUser.UserId);

            // Check if the user profile exists
            if (userProfile == null)
            {
                return NotFound("User profile not found.");
            }
            // Check if User.Identity is null
            if (User?.Identity?.Name == null)
                return Unauthorized(new { Message = "User identity is not available." });

            // Check if user exists
            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(new { Message = "User name is not available." });

            var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (superuser == null)
            {
                return Unauthorized("User not found.");
            }
            var userId = superuser.Id; // Use this as your userId

            // Mark the profile as deleted
            userProfile.Deleted = true;
            userProfile.Published = false;
            userProfile.LastUpdateDate = DateTime.UtcNow;
            userProfile.LastUpdateUser = Guid.Parse(userId); // Assuming the same UserId is the one updating the record.

            // Save changes to the database
            _context.UserProfile.Update(userProfile);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "User profile marked as Deleted", UserProfile = userProfile });
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

    }
}
