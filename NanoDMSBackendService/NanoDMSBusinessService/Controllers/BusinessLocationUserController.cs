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
    public class BusinessLocationUserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IBusinessLocationUserRepository _businessLocationUserRepository;


        #region Constructor
        public BusinessLocationUserController(AppDbContext context,
            IBusinessLocationUserRepository businessLocationUserRepository,
            IConfiguration configuration,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _businessLocationUserRepository = businessLocationUserRepository;
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;

        }
        #endregion

        #region Apis

        [Authorize]
        [HttpPost("register-business-location-user")]
        public async Task<IActionResult> RegisterBusinessLocationUser([FromBody] RegisterBusinessLocationUserModel model)
        {
            try
            {
                if (model.BusinessId == Guid.Empty)
                    return BadRequest(new { Message = "Business Id Is Required." });

                if (model.BusinessLocationIds == null || model.BusinessLocationIds.Count == 0)
                    return BadRequest(new { Message = "At Least One Business Location Id Is Required." });

                if (model.UserId == Guid.Empty)
                    return BadRequest(new { Message = "UserId Is Required." });

                // Check if User.Identity is null
                if (User?.Identity?.Name == null)
                    return Unauthorized(new { Message = "User Identity Is Not available." });

                // Check if user exists
                var userName = User.Identity.Name;
                if (string.IsNullOrEmpty(userName))
                    return Unauthorized(new { Message = "User Name Is Not Available." });

                // Check if the logged-in user exists
                var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
                if (superuser == null)
                    return Unauthorized(new { Message = "User Not Found." });

                var businessLocationUsers = new List<BusinessLocationUser>();

                foreach (var businessLocationId in model.BusinessLocationIds)
                {
                    var businesslocationuser = new BusinessLocationUser
                    {
                        BusinessId = model.BusinessId,
                        BusinessLocationId = businessLocationId,
                        UserId = model.UserId,
                        CreateDate = DateTime.UtcNow,
                        Published = true,
                        CreateUser = Guid.Parse(superuser.Id)
                    };

                    businessLocationUsers.Add(businesslocationuser);
                }

                // Save all records in bulk
                await _businessLocationUserRepository.AddRangeAsync(businessLocationUsers);
                await _businessLocationUserRepository.SaveChangesAsync();

                return Ok(new { Message = "Business Location Users Registered Successfully", BusinessLocationUsers = businessLocationUsers });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = $"Error: {ex.Message}" });
            }
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-business-location-user-list")]
        public async Task<IActionResult> GetBusinessLocationUserList([FromQuery] BusinessFilterModel filter)
        {
            try
            {
                var query = _context.BusinessLocationUser.Select(b => new BusinessLocationUser
                {
                    Id = b.Id,
                    BusinessId = b.BusinessId,
                    BusinessLocationId = b.BusinessLocationId,
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
                var businesslocationuserProfiles = await query
                    .Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToListAsync();

                if (businesslocationuserProfiles.Count == 0)
                {
                    return NoContent();
                }

                // Step 2: Collect Unique business, user,businesslocation IDs
                var businessIds = businesslocationuserProfiles.Select(x => x.BusinessId).Distinct().ToList();
                var businesslocationIds = businesslocationuserProfiles.Select(x => x.BusinessLocationId).Distinct().ToList();
                var userIds = businesslocationuserProfiles.Select(x => x.UserId).Distinct().ToList();

                // Step 3: Fetch Names from External APIs
                var jwtToken = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var apiService = new ApiServiceHelper(new HttpClient());

                if (string.IsNullOrEmpty(jwtToken))
                {
                    return Unauthorized(new { Message = "JWT Token Is Missing Or Invalid." });
                }

                var businessNames = await FetchBusinessNamesAsync(apiService, "Business", "businesse", businessIds, jwtToken);
                var businessLocationNames = await FetchBusinessLocationNamesAsync(apiService, "BusinessLocation", "businesses-location", businesslocationIds, jwtToken);
                var userNames = await FetchUserNamesAsync(apiService, "Profile", "profile", userIds, jwtToken);


                // Step 4: Map the Names to Business Locations
                var result = businesslocationuserProfiles.Select(b => new BusinessLocationUserListModel
                {
                    Id = b.Id,
                    BusinessName = businessNames.TryGetValue(b.BusinessId, out var bName) ? bName : "Unknown",
                    UserName = userNames.TryGetValue(b.UserId, out var uName) ? uName : "Unknown",
                    BusinessLocationName = businessLocationNames.TryGetValue(b.BusinessLocationId, out var blName) ? blName : "Unknown",
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



                var response = new PaginatedResponseDto<BusinessLocationUserListModel>
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

                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-business-location-users")]
        public async Task<IActionResult> GetBusinessLocationUsers()
        {
            try
            {
                var businesslocationuser = await _context.BusinessLocationUser.OrderByDescending(b=> b.CreateDate).ToListAsync();
                if (!businesslocationuser.Any()) return NotFound("No Businesses Location User found!.");

                return Ok(new { BusinessLocationUser = businesslocationuser });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpPost("get-business-location-user-by-user-id")]
        public async Task<IActionResult> GetBusinessLocationUserByUserId(BusinessLocationUserByIdModel businesslocationuserById)
        {
            try
            {
                var businessLocationUsers = await _businessLocationUserRepository
                    .GetAllByConditionAsync(x => x.UserId == businesslocationuserById.UserId);

                if (businessLocationUsers == null || !businessLocationUsers.Any())
                    return NotFound("Business Location User not found.");

                var jwtToken = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (string.IsNullOrEmpty(jwtToken))
                    return Unauthorized(new { Message = "JWT Token Is Missing Or Invalid." });

                var apiService = new ApiServiceHelper(new HttpClient());
                var businessId = businessLocationUsers.First().BusinessId;

                var businessName = await FetchBusinessNameAsync(apiService, "Business", "businesse", businessId, jwtToken);

                // All business location IDs
                var businessLocationIds = businessLocationUsers
                    .Select(x => x.BusinessLocationId)
                    .Distinct()
                    .ToList();

                var businessLocationNames = await FetchBusinessLocationNamesAsync(
                    apiService, "BusinessLocation", "businesses-location", businessLocationIds, jwtToken
                );

                // 🧠 Build list using each matching record
                var businessLocationDtos = businessLocationUsers.Select(user => new BusinessLocationDto
                {
                    BusinessLocationId = user.BusinessLocationId,
                    BusinessLocationName = businessLocationNames.TryGetValue(user.BusinessLocationId, out var name)
                        ? name
                        : "Unknown",

                    // ✅ Now each record gets its own values
                    CreateDate = user.CreateDate,
                    CreateUser = user.CreateUser,
                    LastUpdateDate = user.LastUpdateDate,
                    LastUpdateUser = user.LastUpdateUser,
                    Published = user.Published,
                    Deleted = user.Deleted
                }).ToList();

                var result = new BusinessLocationUserListByIdModel
                {
                    Id = businessLocationUsers.First().Id, // Still okay, assuming one user entry per userId
                    BusinessId = businessId,
                    BusinessName = businessName,
                    UserId = businesslocationuserById.UserId,
                    BusinessLocations = businessLocationDtos
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpPost("get-business-location-user-by-id")]
        public async Task<IActionResult> GetBusinessLocationUserById(BusinessLocationUserByIdModel businesslocationuserById)
        {
            try
            {
                var businesslocationuser = await _businessLocationUserRepository.GetByIdAsync(businesslocationuserById.Id);

                return businesslocationuser == null ? NotFound("Business Location User Not Found.") : Ok(new { BusinessLocationUser = businesslocationuser });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPut("edit-business-location-user")]
        public async Task<IActionResult> EditBusinessLocationUser([FromBody] UpdateBusinessLocationUserModel updateDto)
        {
            try
            {
                // Validate UserId
                if (updateDto.UserId == Guid.Empty)
                    return BadRequest(new { Message = "User Id Is Required." });

                // Validate BusinessId
                if (updateDto.BusinessId == Guid.Empty)
                    return BadRequest(new { Message = "Business Id Is Required." });

                // Validate Business Location Id
                if (updateDto.BusinessLocationIds == null || updateDto.BusinessLocationIds.Count == 0)
                    return BadRequest(new { Message = "Atleast One Business Location Id Is Required." });

                // Check if User.Identity is null
                if (User?.Identity?.Name == null)
                    return Unauthorized(new { Message = "User Identity Is Not Available." });

                // Check if user exists
                var userName = User.Identity.Name;
                if (string.IsNullOrEmpty(userName))
                    return Unauthorized(new { Message = "User Name Is Not Available." });


                var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
                if (superuser == null) return Unauthorized("User Not Found.");

                // Delete existing records for the UserId
                var existingRecords = await _businessLocationUserRepository.GetByUserIdAsync(updateDto.UserId);
                if (existingRecords != null && existingRecords.Any())
                {
                    foreach (var record in existingRecords)
                    {
                        _businessLocationUserRepository.Delete(record);
                    }
                    await _businessLocationUserRepository.SaveChangesAsync();
                }
                var businessLocationUsers = new List<BusinessLocationUser>();

                foreach (var businessLocationId in updateDto.BusinessLocationIds)
                {
                    var businesslocationuser = new BusinessLocationUser
                    {
                        BusinessId = updateDto.BusinessId,
                        BusinessLocationId = businessLocationId,
                        UserId = updateDto.UserId,
                        CreateDate = DateTime.UtcNow,
                        Published = true,
                        CreateUser = Guid.Parse(superuser.Id),
                        LastUpdateDate = DateTime.UtcNow,
                        LastUpdateUser = Guid.Parse(superuser.Id)
                    };

                    businessLocationUsers.Add(businesslocationuser);
                }


                // Insert new record
                await _businessLocationUserRepository.AddRangeAsync(businessLocationUsers);
                await _businessLocationUserRepository.SaveChangesAsync();

                // Return success response
                return Ok(new
                {
                    Message = "Business Location User Updated Successfully!",
                    BusinessLocationUser = businessLocationUsers
                });
            }
            catch (Exception ex)
            {
                // Handle server errors
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = $"Error: {ex.Message}" });
            }
        }

        [Authorize]
        [HttpDelete("delete-business-location-user")]
        public async Task<IActionResult> DeleteBusinessLocationUser(DeleteBusinessLocationUserModel deleteBusinessLocationuser)
        {
            var businesslocationuser = await _businessLocationUserRepository.GetByIdAsync(deleteBusinessLocationuser.Id);
            if (businesslocationuser == null) return NotFound("Business Location User Not Found.");

            // Check if User.Identity is null
            if (User?.Identity?.Name == null)
                return Unauthorized(new { Message = "User Identity Is Not Available." });

            // Check if user exists
            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(new { Message = "User Name Is Not Available." });


            var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (superuser == null) return Unauthorized("User Not Found.");

            businesslocationuser.Deleted = true;
            businesslocationuser.Published = false;
            businesslocationuser.LastUpdateDate = DateTime.UtcNow;
            businesslocationuser.LastUpdateUser = Guid.Parse(superuser.Id);

            _businessLocationUserRepository.Update(businesslocationuser);
            await _businessLocationUserRepository.SaveChangesAsync();

            return Ok(new { Message = "Business Location User Marked As Deleted", BusinessLocationUser = businesslocationuser });
        }

        #endregion

        #region Function
        

        private async Task<string> FetchBusinessNameAsync(ApiServiceHelper apiService, string locationType, string actionType, Guid id, string jwtToken)
        {
            var apiUrl = $"http://localhost:8010/apigateway/BusinessService/{locationType}/get-{actionType.ToLower()}s";

           // var apiUrl = $"http://192.168.100.61/apigateway/BusinessService/{locationType}/get-{actionType.ToLower()}s";

            var response = await apiService.SendRequestAsync<object, Dictionary<string, object>>(apiUrl, HttpMethod.Get, null, jwtToken)
                           ?? new Dictionary<string, object>();

            if (response.TryGetValue(locationType.ToLower(), out var jsonElementObj) && jsonElementObj is JsonElement jsonElement)
            {
                var locationList = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(
                    jsonElement.GetRawText(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                ) ?? new List<Dictionary<string, object>>();

        var business = locationList.FirstOrDefault(b => b.TryGetValue("id", out var idValue) && Guid.TryParse(idValue?.ToString(), out var parsedId) && parsedId == id);
                if (business != null && business.TryGetValue("name", out var nameValue))
                {
                    return nameValue?.ToString() ?? string.Empty; // Ensures no null reference return
                }
}

return string.Empty; // Ensures no null reference return
        }

        private async Task<Dictionary<Guid, string>> FetchBusinessLocationNamesAsync(
    ApiServiceHelper apiService,
    string locationType,
    string actionType,
    List<Guid> ids,
    string jwtToken)
        {
            if (ids == null || ids.Count == 0)
                return new Dictionary<Guid, string>();

            var apiUrl = $"http://localhost:8010/apigateway/BusinessService/{locationType}/get-{actionType.ToLower()}s";

            //var apiUrl = $"http://192.168.100.61/apigateway/BusinessService/{locationType}/get-{actionType.ToLower()}s";

            // Send GET request expecting JSON response
            var response = await apiService.SendRequestAsync<object, Dictionary<string, object>>(
                apiUrl, HttpMethod.Get, null, jwtToken) ?? new Dictionary<string, object>();

            // Extract business location data from response
            if (response.TryGetValue("businessLocations", out var jsonElementObj) && jsonElementObj is JsonElement jsonElement)
            {
                var locationList = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(
                    jsonElement.GetRawText(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                ) ?? new List<Dictionary<string, object>>();

                // Map locations to a dictionary of ID -> Name
                return locationList
                    .Where(loc => loc.ContainsKey("id") && loc.ContainsKey("name"))
                    .ToDictionary(
                        loc => Guid.Parse(loc["id"]?.ToString() ?? string.Empty),
                        loc => loc["name"]?.ToString() ?? string.Empty // Ensure null-safe conversion
                    );
            }

            return new Dictionary<Guid, string>();
        }

        private async Task<Dictionary<Guid, string>> FetchBusinessNamesAsync(ApiServiceHelper apiService, string locationType, string actiontype, List<Guid> ids, string jwtToken)
        {
            if (ids == null || ids.Count == 0)
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
                        c => Guid.Parse(c["id"]?.ToString() ?? string.Empty),
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

            

            if (response == null) return new Dictionary<Guid, string>();

            // Convert response to Dictionary<Guid, string>
            return response.ToDictionary(
                user => Guid.Parse(user["userId"].ToString()?? string.Empty),  // Ensure key exists
                user => $"{user["userName"]} " // Ensure both keys exist
            );
        }

        #endregion

    }
}
