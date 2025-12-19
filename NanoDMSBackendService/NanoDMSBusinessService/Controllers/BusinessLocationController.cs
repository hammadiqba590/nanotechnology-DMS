using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NanoDMSBusinessService.Data;
using NanoDMSBusinessService.DTO;
using NanoDMSBusinessService.Models;
using NanoDMSBusinessService.Repositories;
using NanoDMSSharedLibrary;
using System.Net;
using System.Numerics;
using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;

namespace NanoDMSBusinessService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessLocationController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IBusinessLocationRepository _businessLocationRepository;

        #region Constructor
        public BusinessLocationController(AppDbContext context,
            IBusinessLocationRepository businessLocation,
            IConfiguration configuration,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _businessLocationRepository = businessLocation;
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;

        }
        #endregion

        #region Apis

        [Authorize]
        [HttpPost("register-business-location")]
        public async Task<IActionResult> RegisterBusinessLocation([FromBody] RegisterBusinessLocationModel model)
        {
            try
            {

                if (string.IsNullOrEmpty(model.Name))
                    return BadRequest(new { Message = "Business Location Name is required." });


                if (model.BusinessId == Guid.Empty)
                    return BadRequest(new { Message = "Business Id is required." });


                if (string.IsNullOrEmpty(model.Address))
                    return BadRequest(new { Message = "Business Address is required." });


                if (model.Country == Guid.Empty)
                    return BadRequest(new { Message = "Business Country is required." });

                if (model.State == Guid.Empty)
                    return BadRequest(new { Message = "Business State is required." });

                if (model.City == Guid.Empty)
                    return BadRequest(new { Message = "Business City is required." });

                if (string.IsNullOrEmpty(model.PostalCode))
                    return BadRequest(new { Message = "Business Postal Code is required." });

                if (string.IsNullOrEmpty(model.Phone))
                    return BadRequest(new { Message = "Business Phone Number  is required." });

                if (string.IsNullOrEmpty(model.Mobile))
                    return BadRequest(new { Message = "Business Mobile Number is required." });

                if (string.IsNullOrEmpty(model.Email))
                    return BadRequest(new { Message = "Business Email is required." });

                // 🔥 UNIQUE NAME CHECK
                var normalizedName = model.Name.Trim().ToUpper();

                // 🔥 ADD UNIQUENESS CHECK HERE

                var existingLocation = await _context.BusinessLocation
            .Where(x => x.BusinessId == model.BusinessId &&
                        x.Name.Trim().ToUpper() == normalizedName &&
                        !x.Deleted)
            .FirstOrDefaultAsync();

                if (existingLocation != null)
                {
                    return BadRequest(new { Message = "Business Location Name must be unique for this Business." });
                }

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

                // Map DTO to entity (Assuming there's a Business entity)
                var businesslocation = new BusinessLocation
                {
                    Name = model.Name.Trim(),
                    BusinessId = model.BusinessId,
                    Address = model.Address,
                    Country = model.Country,
                    State = model.State,
                    City = model.City,
                    PostalCode = model.PostalCode,
                    Phone = model.Phone,
                    Mobile = model.Mobile,
                    Email = model.Email,
                    Website = model.Website,
                    CreateDate = DateTime.UtcNow,
                    Published = true,
                    CreateUser = Guid.Parse(superuser.Id)
                };

                // Save to repository
                await _businessLocationRepository.AddAsync(businesslocation);
                await _businessLocationRepository.SaveChangesAsync();

                // Return success response
                return Ok(new { Message = "Business Location registered successfully", BusinessLocation = businesslocation });
            }
            catch (Exception ex)
            {
                // Handle server errors
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = $"Error: {ex.Message}" });
            }
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-business-location-list")]
        public async Task<IActionResult> GetBusinessLocationList([FromQuery] BusinessFilterModel filter)
        {
            try
            {
                // Step 1: Build the query with Business name
                var query = from b in _context.BusinessLocation
                            join bs in _context.Business on b.BusinessId equals bs.Id into bsJoin
                            from bs in bsJoin.DefaultIfEmpty()
                            select new
                            {
                                b.Id,
                                b.BusinessId,
                                BusinessName = bs != null ? bs.Name : "Unknown",
                                b.Name,
                                b.Address,
                                b.Country,
                                b.State,
                                b.City,
                                b.PostalCode,
                                b.Phone,
                                b.Mobile,
                                b.Email,
                                b.Website,
                                b.CreateDate,
                                b.CreateUser,
                                b.LastUpdateDate,
                                b.LastUpdateUser,
                                b.Published,
                                b.Deleted,
                            };

                // Step 2: Apply filtering
                if (!string.IsNullOrEmpty(filter.Name))
                    query = query.Where(q => q.Name.Contains(filter.Name));

                // Sort by CreateDate descending to get the latest profiles first
                query = query.OrderByDescending(q => q.CreateDate);

                // Step 3: Get total records and apply pagination
                var totalRecords = await query.CountAsync();

                var businessLocations = await query
                    .Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToListAsync();

                if (!businessLocations.Any())
                    return NoContent();

                // Step 4: Collect unique location IDs
                var countryIds = businessLocations.Select(x => x.Country).Distinct().ToList();
                var stateIds = businessLocations.Select(x => x.State).Distinct().ToList();
                var cityIds = businessLocations.Select(x => x.City).Distinct().ToList();

                // Step 5: Get auth token
                var jwtToken = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (string.IsNullOrEmpty(jwtToken))
                    return Unauthorized(new { Message = "Authorization token is missing or invalid." });

                // Step 6: Fetch external location names
                var apiService = new ApiServiceHelper(new HttpClient());

                var countryNames = await FetchLocationNamesAsync(apiService, "Country", "countrie", countryIds, jwtToken);
                var stateNames = await FetchLocationNamesAsync(apiService, "State", "state", stateIds, jwtToken);
                var cityNames = await FetchLocationNamesAsync(apiService, "City", "citie", cityIds, jwtToken);

                // Step 7: Map to DTO list
                var result = businessLocations.Select(b => new BusinessLocationListModel
                {
                    Id = b.Id,
                    BusinessId = b.BusinessId,
                    BusinessName = b.BusinessName,
                    Name = b.Name,
                    Address = b.Address,
                    Country = countryNames.TryGetValue(b.Country, out var cName) ? cName : "Unknown",
                    State = stateNames.TryGetValue(b.State, out var sName) ? sName : "Unknown",
                    City = cityNames.TryGetValue(b.City, out var ctName) ? ctName : "Unknown",
                    PostalCode = b.PostalCode,
                    Phone = b.Phone,
                    Mobile = b.Mobile,
                    Email = b.Email,
                    Website = b.Website,
                    CreateDate = b.CreateDate,
                    CreateUser = b.CreateUser,
                    LastUpdateDate = b.LastUpdateDate,
                    LastUpdateUser = b.LastUpdateUser,
                    Published = b.Published,
                    Deleted = b.Deleted,
                }).OrderByDescending(b=> b.CreateDate).ToList();

                // Step 8: Return paginated response
                var response = new PaginatedResponseDto<BusinessLocationListModel>
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
        [HttpPost("get-business-location-count")]
        public async Task<IActionResult> GetBusinessLocationCount(BusinessLocationCountModel businessLocationCountModel)
        {
            try
            {
                // Check if the businessId is valid
                if (Guid.Parse(businessLocationCountModel.BusinessId) == Guid.Empty)
                    return BadRequest(new { Message = "Invalid Business ID." });

                // Get the count of locations for the given businessId
                int locationCount = await _businessLocationRepository.GetLocationCountByBusinessIdAsync(Guid.Parse(businessLocationCountModel.BusinessId));
                if (locationCount > 0)
                {
                    // Return the location count
                    return Ok(new { BusinessId = Guid.Parse(businessLocationCountModel.BusinessId), LocationCount = locationCount });
                }
                else
                {
                    // Return the location count
                    return Ok(new { Message = "No Location Found For This Business", LocationCount = locationCount });
                }

            }
            catch (Exception ex)
            {
                // Handle errors
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = $"Error: {ex.Message}" });
            }
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-businesses-locations")]
        public async Task<IActionResult> GetBusinessesLocations()
        {
            try
            {
                var locations = await _businessLocationRepository.GetAllWithBusinessAsync();

                if (locations.Count == 0)
                    return NotFound("No Business Locations found.");

                var dtos = locations.Select(loc => new BusinessLocationsDto
                {
                    Id = loc.Id,
                    BusinessId = loc.BusinessId,
                    BusinessName = loc.Business?.Name ?? string.Empty,
                    Name = loc.Name,
                    Address = loc.Address,
                    Country = loc.Country,
                    State = loc.State,
                    City = loc.City,
                    PostalCode = loc.PostalCode,
                    Phone = loc.Phone,
                    Mobile = loc.Mobile,
                    Email = loc.Email,
                    Website = loc.Website,
                    Deleted = loc.Deleted,
                    Published = loc.Published,
                    CreateDate = loc.CreateDate,
                    CreateUser = loc.CreateUser,
                    LastUpdateDate = loc.LastUpdateDate,
                    LastUpdateUser = loc.LastUpdateUser
                }).OrderByDescending(dto => dto.CreateDate)
                .ToList();

                // wrap the list however you want; here I’m keeping the same wrapper key
                return Ok(new { BusinessLocations = dtos });
            }
            catch (Exception ex)
            {
                // log ex as usual
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpPost("get-business-location-by-id")]
        public async Task<IActionResult> GetBusinessLocationById(BusinessLocationByIdModel model)
        {
            try
            {
                var location = await _businessLocationRepository
                                     .GetByIdWithBusinessAsync(Guid.Parse(model.Id));

                if (location is null)
                    return NotFound("Business Location not found.");

                var dto = new BusinessLocationsDto
                {
                    Id = location.Id,
                    BusinessId = location.BusinessId,
                    BusinessName = location.Business?.Name ?? string.Empty,
                    Name = location.Name,
                    Address = location.Address,
                    Country = location.Country,
                    State = location.State,
                    City = location.City,
                    PostalCode = location.PostalCode,
                    Mobile = location.Mobile,
                    Phone = location.Phone,
                    Email = location.Email,
                    Website = location.Website,
                };


                return Ok(new { BusinessLocation = dto });
            }
            catch (Exception ex)
            {
                // log the exception here if you aren’t already
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPut("edit-business-location")]
        public async Task<IActionResult> EditBusinessLocation([FromBody] UpdateBusinessLocationModel updateDto)
        {
            try
            {
                // Validate Id
                if (string.IsNullOrEmpty(updateDto.Id))
                    return BadRequest(new { Message = "Business Location Id is required." });

                if (string.IsNullOrEmpty(updateDto.BusinessId))
                    return BadRequest(new { Message = "Business Id is required." });

                // Validate Name
                if (string.IsNullOrEmpty(updateDto.Name))
                    return BadRequest(new { Message = "Business Location Name is required." });

                // Check if the business exists
                var businesslocation = await _businessLocationRepository.GetByIdAsync(Guid.Parse(updateDto.Id));
                if (businesslocation == null)
                    return NotFound(new { Message = "Business Location not found!." });

                // Check if User.Identity is null
                if (User?.Identity?.Name == null)
                    return Unauthorized(new { Message = "User identity is not available." });

                // Check if user exists
                var userName = User.Identity.Name;
                if (string.IsNullOrEmpty(userName))
                    return Unauthorized(new { Message = "User name is not available." });

                var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
                if (superuser == null) return Unauthorized("User not found.");

                // 🔥 UNIQUE NAME CHECK (EXCEPT CURRENT RECORD)
                var normalizedName = updateDto.Name.Trim().ToUpper();
                var businessId = Guid.Parse(updateDto.BusinessId);

                var duplicate = await _context.BusinessLocation
                    .Where(x =>
                        x.BusinessId == businessId &&
                        x.Id != businesslocation.Id &&                // Exclude current
                        x.Name.Trim().ToUpper() == normalizedName &&
                        !x.Deleted)
                    .FirstOrDefaultAsync();

                if (duplicate != null)
                {
                    return BadRequest(new { Message = "Another Business Location with this Name already exists for this Business." });
                }


                // Update the business properties
                businesslocation.Name = updateDto.Name.Trim();
                businesslocation.BusinessId = Guid.Parse(updateDto.BusinessId);
                businesslocation.Address = !string.IsNullOrEmpty(updateDto.Address) ? updateDto.Address : businesslocation.Address;
                businesslocation.Country = !string.IsNullOrEmpty(updateDto.Country) ? Guid.Parse(updateDto.Country) : businesslocation.Country;
                businesslocation.State = string.IsNullOrWhiteSpace(updateDto.State) ? businesslocation.State : Guid.Parse(updateDto.State);
                businesslocation.City = !string.IsNullOrEmpty(updateDto.City) ? Guid.Parse(updateDto.City) : businesslocation.City;
                businesslocation.PostalCode = !string.IsNullOrEmpty(updateDto.PostalCode) ? updateDto.PostalCode : businesslocation.PostalCode;
                businesslocation.Phone = !string.IsNullOrEmpty(updateDto.Phone) ? updateDto.Phone : businesslocation.Phone;
                businesslocation.Mobile = !string.IsNullOrEmpty(updateDto.Mobile) ? updateDto.Mobile : businesslocation.Mobile;
                businesslocation.Email = !string.IsNullOrEmpty(updateDto.Email) ? updateDto.Email : businesslocation.Email;
                businesslocation.Website = !string.IsNullOrEmpty(updateDto.Website) ? updateDto.Website : businesslocation.Website;
                businesslocation.LastUpdateDate = DateTime.UtcNow;
                businesslocation.Published = true;
                businesslocation.LastUpdateUser = Guid.Parse(superuser.Id);

                // Save updates
                _businessLocationRepository.Update(businesslocation);
                await _businessLocationRepository.SaveChangesAsync();

                // Return success response
                return Ok(new
                {
                    Message = "Business Location updated successfully!",
                    BusinessLocation = businesslocation
                });
            }
            catch (Exception ex)
            {
                // Handle server errors
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = $"Error: {ex.Message}" });
            }
        }

        [Authorize]
        [HttpDelete("delete-business-location")]
        public async Task<IActionResult> DeleteBusinessLocation(DeleteBusinessLocationModel deleteBusinessLocation)
        {
            var businesslocation = await _businessLocationRepository.GetByIdAsync(Guid.Parse(deleteBusinessLocation.Id));
            if (businesslocation == null) return NotFound("Business Location not found.");

            // Check if User.Identity is null
            if (User?.Identity?.Name == null)
                return Unauthorized(new { Message = "User identity is not available." });

            // Check if user exists
            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(new { Message = "User name is not available." });

            var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (superuser == null) return Unauthorized("User not found.");

            businesslocation.Deleted = true;
            businesslocation.Published = false;
            businesslocation.LastUpdateDate = DateTime.UtcNow;
            businesslocation.LastUpdateUser = Guid.Parse(superuser.Id);

            _businessLocationRepository.Update(businesslocation);
            await _businessLocationRepository.SaveChangesAsync();

            return Ok(new { Message = "Business Location marked as Deleted", BusinessLocation = businesslocation });
        }
        #endregion

        #region Function
        private async Task<Dictionary<Guid, string>> FetchLocationNamesAsync(ApiServiceHelper apiService, string locationType, string actiontype,List<Guid> ids, string jwtToken)
        {
            if (ids == null || !ids.Any())
                return new Dictionary<Guid, string>();

            //var apiUrl = $"http://192.168.100.61/apigateway/SetupService/{locationType}/get-{actiontype.ToLower()}s";

            var apiUrl = $"http://localhost:8010/apigateway/SetupService/{locationType}/get-{actiontype.ToLower()}s";

            var response = await apiService.SendRequestAsync<object, Dictionary<string, object>>(apiUrl, HttpMethod.Get, ids, jwtToken)
                           ?? new Dictionary<string, object>();

            if (response.TryGetValue(locationType.ToLower(), out var jsonElementObj) && jsonElementObj is JsonElement jsonElement)
            {
                var locationList = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(
                    jsonElement.GetRawText(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                ) ?? new List<Dictionary<string, object>>();

                return locationList
                    .Where(c => c.ContainsKey("id") && c.ContainsKey("name") && c["id"] != null && c["name"] != null)
                    .ToDictionary(c => Guid.TryParse(c["id"]?.ToString(), out var parsedId) ? parsedId : Guid.Empty,
                        c => c["name"]!.ToString() ?? "Unknown"
                    );
            }

            return new Dictionary<Guid, string>();
        }

        #endregion
    }
}
