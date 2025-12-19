using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NanoDMSBusinessService.Data;
using NanoDMSBusinessService.DTO;
using NanoDMSBusinessService.Models;
using NanoDMSBusinessService.Repositories;
using NanoDMSSharedLibrary;

namespace NanoDMSBusinessService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessUserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IBusinessUserRepository _businessUserRepository;

        #region Constuctor
        public BusinessUserController(AppDbContext context,
            IBusinessUserRepository businessUserRepository,
            IConfiguration configuration,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _businessUserRepository = businessUserRepository;
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;

        }
        #endregion

        #region Apis

        [Authorize]
        [HttpPost("register-business-user")]
        public async Task<IActionResult> RegisterBusinessUser([FromBody] RegisterBusinessUserModel model)
        {
            try
            {

                if (model.BusinessId == Guid.Empty)
                    return BadRequest(new { Message = "Business Id is required." });


                if (model.UserID == Guid.Empty)
                    return BadRequest(new { Message = "UserId is required." });


                // Check if User.Identity is null
                if (User?.Identity?.Name == null)
                    return Unauthorized(new { Message = "User identity is not available." });

                // Check if user exists
                var userName = User.Identity.Name;
                if (string.IsNullOrEmpty(userName))
                    return Unauthorized(new { Message = "User name is not available." });


                // Check if user exists
                var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
                if (superuser == null)
                    return Unauthorized(new { Message = "User not found." });


                var businessuser = new BusinessUser
                {
                    BusinessId = model.BusinessId,
                    UserId = model.UserID,
                    CreateDate = DateTime.UtcNow,
                    Published = true,
                    CreateUser = Guid.Parse(superuser.Id)
                };

                // Save to repository
                await _businessUserRepository.AddAsync(businessuser);
                await _businessUserRepository.SaveChangesAsync();

                // Return success response
                return Ok(new { Message = "Business User registered successfully", BusinessUser = businessuser });
            }
            catch (Exception ex)
            {
                // Handle server errors
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = $"Error: {ex.Message}" });
            }
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-business-user-list")]
        public async Task<IActionResult> GetBusinessUserList([FromQuery] BusinessFilterModel filter)
        {
            try
            {
                var query = _context.BusinessUser.Select(b => new BusinessUser
                {
                    Id = b.Id,
                    BusinessId = b.BusinessId,
                    UserId = b.UserId,
                    CreateDate = b.CreateDate,
                    CreateUser = b.CreateUser,
                    LastUpdateDate = b.LastUpdateDate,
                    LastUpdateUser = b.LastUpdateUser,
                    Published = b.Published,
                    Deleted = b.Deleted,

                }).AsQueryable();



                // Filter by BusinessId
                if (filter.BusinessId != Guid.Empty)
                    query = query.Where(q => q.BusinessId == filter.BusinessId);

                // Filter by BusinessId
                if (filter.UserId != Guid.Empty)
                    query = query.Where(q => q.UserId == filter.UserId);

                // Sort by CreateDate descending to get the latest profiles first
                query = query.OrderByDescending(q => q.CreateDate);

                // **Apply Pagination**
                var totalRecords = await query.CountAsync();
                // Apply pagination
                var businessuserProfiles = await query
                    .Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToListAsync();

                if (!businessuserProfiles.Any())
                {
                    return NoContent();
                }
                // Step 2: Collect Unique business, user IDs
                var businessIds = businessuserProfiles.Select(x => x.BusinessId).Distinct().ToList();
                var userIds = businessuserProfiles.Select(x => x.UserId).Distinct().ToList();


                // Step 3: Fetch Names from External APIs
                var jwtToken = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var apiService = new ApiServiceHelper(new HttpClient());

                if (string.IsNullOrEmpty(jwtToken))
                {
                    return Unauthorized(new { Message = "JWT token is missing or invalid." });
                }

                var businessNames = await FetchBusinessNamesAsync(apiService, "Business", "businesse", businessIds, jwtToken);
                var userNames = await FetchUserNamesAsync(apiService, "Profile", "profile", userIds, jwtToken);

                // Step 4: Map the Names to Business Locations
                var result = businessuserProfiles.Select(b => new BusinessUserListModel
                {
                    Id = b.Id,
                    BusinessId = b.BusinessId,
                    BusinessName = businessNames.TryGetValue(b.BusinessId, out var bName) ? bName : "Unknown",
                    UserName = userNames.TryGetValue(b.UserId, out var uName) ? uName : "Unknown",
                    CreateDate = b.CreateDate,
                    CreateUser = b.CreateUser,
                    LastUpdateDate = b.LastUpdateDate,
                    LastUpdateUser = b.LastUpdateUser,
                    Published = b.Published,
                    Deleted = b.Deleted,
                }).OrderByDescending(b=> b.CreateDate).ToList();


                // Apply Name filter SAFELY
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    result = result
                        .Where(q => !string.IsNullOrEmpty(q.UserName) &&
                                    q.UserName.Contains(filter.Name, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                var response = new PaginatedResponseDto<BusinessUserListModel>
                {
                    TotalRecords = totalRecords,
                    PageNumber = filter.PageNumber,
                    PageSize = filter.PageSize,
                    TotalPages = (int)Math.Ceiling((double)totalRecords / filter.PageSize),
                    Data = result
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
        [HttpGet("get-business-users")]
        public async Task<IActionResult> GetBusinessUsers()
        {
            try
            {
                var businessuser = await _context.BusinessUser.OrderByDescending(b=> b.CreateDate).ToListAsync();
                if (!businessuser.Any()) return NotFound("No Businesses User found!.");

                return Ok(new { BusinessUser = businessuser });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpPost("get-business-user-by-id")]
        public async Task<IActionResult> GetBusinessUserById(BusinessUserByIdModel businessuserById)
        {
            try
            {
                var businessuser = await _businessUserRepository.GetByIdAsync(businessuserById.Id);

                return businessuser == null ? NotFound("Business User not found.") : Ok(new { BusinessUser = businessuser });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPut("edit-business-user")]
        public async Task<IActionResult> EditBusinessUser([FromBody] UpdateBusinessUserModel updateDto)
        {
            try
            {
                // Validate Id
                if (updateDto.Id == Guid.Empty)
                    return BadRequest(new { Message = "Business User Id is required." });

                // Validate BusinessId
                if (updateDto.BusinessId == Guid.Empty)
                    return BadRequest(new { Message = "Business Id is required." });

                // Validate BusinessId
                if (updateDto.UserId == Guid.Empty)
                    return BadRequest(new { Message = "User Id is required." });

                // Check if the business exists
                var businessuser = await _businessUserRepository.GetByIdAsync(updateDto.Id);
                if (businessuser == null)
                    return NotFound(new { Message = "Business User not found!." });

                // Check if User.Identity is null
                if (User?.Identity?.Name == null)
                    return Unauthorized(new { Message = "User identity is not available." });

                // Check if user exists
                var userName = User.Identity.Name;
                if (string.IsNullOrEmpty(userName))
                    return Unauthorized(new { Message = "User name is not available." });

                var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
                if (superuser == null) return Unauthorized("User not found.");

                // Update the business properties
                businessuser.BusinessId = updateDto.BusinessId;
                businessuser.UserId = updateDto.UserId;
                businessuser.LastUpdateDate = DateTime.UtcNow;
                businessuser.Published = true;
                businessuser.LastUpdateUser = Guid.Parse(superuser.Id);

                // Save updates
                _businessUserRepository.Update(businessuser);
                await _businessUserRepository.SaveChangesAsync();

                // Return success response
                return Ok(new
                {
                    Message = "Business User updated successfully!",
                    BusinessUser = businessuser
                });
            }
            catch (Exception ex)
            {
                // Handle server errors
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = $"Error: {ex.Message}" });
            }
        }


        [Authorize]
        [HttpDelete("delete-business-user")]
        public async Task<IActionResult> DeleteBusinessUser(DeleteBusinessUserModel deleteBusinessuser)
        {
            var businessuser = await _businessUserRepository.GetByIdAsync(deleteBusinessuser.Id);
            if (businessuser == null) return NotFound("BusinessUser not found.");

            // Check if User.Identity is null
            if (User?.Identity?.Name == null)
                return Unauthorized(new { Message = "User identity is not available." });

            // Check if user exists
            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(new { Message = "User name is not available." });


            var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (superuser == null) return Unauthorized("User not found.");

            businessuser.Deleted = true;
            businessuser.Published = false;
            businessuser.LastUpdateDate = DateTime.UtcNow;
            businessuser.LastUpdateUser = Guid.Parse(superuser.Id);

            _businessUserRepository.Update(businessuser);
            await _businessUserRepository.SaveChangesAsync();

            return Ok(new { Message = "Business User marked as Deleted", BusinessUser = businessuser });
        }
        #endregion

        #region Function
        private async Task<Dictionary<Guid, string>> FetchBusinessNamesAsync(ApiServiceHelper apiService, string locationType, string actiontype, List<Guid> ids, string jwtToken)
        {
            if (ids == null || !ids.Any())
                return new Dictionary<Guid, string>();

            var apiUrl = $"http://localhost:8010/apigateway/BusinessService/{locationType}/get-{actiontype.ToLower()}s";

            //var apiUrl = $"http://192.168.100.61/apigateway/BusinessService/{locationType}/get-{actiontype.ToLower()}s";

            var response = await apiService.SendRequestAsync<object, Dictionary<string, object>>(apiUrl, HttpMethod.Get, ids, jwtToken)
                           ?? new Dictionary<string, object>();

            if (response.TryGetValue(locationType.ToLower(), out var jsonElementObj) && jsonElementObj is JsonElement jsonElement)
            {
                var locationList = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(
                    jsonElement.GetRawText(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                ) ?? new List<Dictionary<string, object>>();

#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
                return locationList
                    .Where(c => c.ContainsKey("id") && c.ContainsKey("name"))
                    .ToDictionary(
                        c => Guid.Parse(c["id"].ToString() ?? string.Empty),
                        c => c["name"].ToString()
                    );
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
            }

            return new Dictionary<Guid, string>();
        }
        private async Task<Dictionary<Guid, string>> FetchUserNamesAsync(
    ApiServiceHelper apiService,
    string service,
    string endpoint,
    List<Guid> userIds,
    string jwtToken)
        {
            var response = await apiService.SendRequestAsync<List<Guid>, List<Dictionary<string, object>>>(
                $"http://localhost:8010/apigateway/AuthService/{service}/get-{endpoint.ToLower()}",
                HttpMethod.Get,
                userIds,
                jwtToken
            );

            //var response = await apiService.SendRequestAsync<List<Guid>, List<Dictionary<string, object>>>(
            //    $"http://192.168.100.61/apigateway/AuthService/{service}/get-{endpoint.ToLower()}",
            //    HttpMethod.Get,
            //    userIds,
            //    jwtToken
            //);

            if (response == null) return new Dictionary<Guid, string>();

            // Convert response to Dictionary<Guid, string>
            return response
                .Where(user => user.ContainsKey("userId") && user["userId"] != null) // Ensure key exists and is not null
                .ToDictionary(
                    user => Guid.Parse(user["userId"]?.ToString() ?? string.Empty),  // Safely handle null values
                    user => $"{user["userName"]}" // Ensure both keys exist
                );
        }

        #endregion
    }
}
