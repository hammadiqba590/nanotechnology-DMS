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
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 🔹 VALIDATIONS
                if (string.IsNullOrWhiteSpace(model.Name))
                    return BadRequest(new { Message = "Business Location Name Is Required." });

                if (model.BusinessId == Guid.Empty)
                    return BadRequest(new { Message = "Business Id Is Required." });

                if (string.IsNullOrWhiteSpace(model.Address))
                    return BadRequest(new { Message = "Business Address Is Required." });

                if (model.Country == Guid.Empty)
                    return BadRequest(new { Message = "Business Country Is Required." });

                if (model.State == Guid.Empty)
                    return BadRequest(new { Message = "Business State Is Required." });

                if (model.City == Guid.Empty)
                    return BadRequest(new { Message = "Business City Is Required." });

                if (string.IsNullOrWhiteSpace(model.PostalCode))
                    return BadRequest(new { Message = "Business Postal Code Is Required." });

                if (string.IsNullOrWhiteSpace(model.Phone))
                    return BadRequest(new { Message = "Business Phone Number Is Required." });

                if (string.IsNullOrWhiteSpace(model.Mobile))
                    return BadRequest(new { Message = "Business Mobile Number Is Required." });

                if (string.IsNullOrWhiteSpace(model.Email))
                    return BadRequest(new { Message = "Business Email Is Required." });

                // 🔹 UNIQUE NAME CHECK
                var normalizedName = model.Name.Trim().ToUpper();

                var existingLocation = await _context.BusinessLocation
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.Business_Id == model.BusinessId &&
                        x.Name.Trim().ToUpper() == normalizedName &&
                        !x.Deleted);

                if (existingLocation != null)
                    return BadRequest(new { Message = "Business Location Name Must Be Unique For This Business." });

                // 🔹 USER CHECK
                if (User?.Identity?.Name == null)
                    return Unauthorized(new { Message = "User Identity Is Not Available." });

                var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
                if (superuser == null)
                    return Unauthorized(new { Message = "User Not Found." });

                // 🔹 CREATE BUSINESS LOCATION
                var businessLocation = new BusinessLocation
                {
                    Name = model.Name.Trim(),
                    Business_Id = model.BusinessId,
                    Address = model.Address,
                    Country = model.Country,
                    State = model.State,
                    City = model.City,
                    Postal_Code = model.PostalCode,
                    Phone = model.Phone,
                    Mobile = model.Mobile,
                    Email = model.Email,
                    Website = model.Website,
                    DiscountBeforeTax = model.DiscountBeforeTax,
                    PosCharge = model.PosCharge,
                    DiscountBeforePosCharge = model.DiscountBeforePosCharge,
                    ServiceCharges = model.ServiceCharges,
                    DiscountBeforeServiceCharge = model.DiscountBeforeServiceCharge,
                    Create_Date = DateTime.UtcNow,
                    Published = true,
                    Create_User = Guid.Parse(superuser.Id)
                };

                await _businessLocationRepository.AddAsync(businessLocation);
                await _businessLocationRepository.SaveChangesAsync();

                // 🔹 INSERT PSPs
                if (model.Psp_Ids?.Any() == true)
                {
                    var psps = model.Psp_Ids.Select(pspId => new BusinessLocationPsp
                    {
                        Id = Guid.NewGuid(),
                        Business_Location_Id = businessLocation.Id,
                        Psp_Id = pspId,
                        Create_Date = DateTime.UtcNow,
                        Published = true,
                        Create_User = Guid.Parse(superuser.Id)
                    });

                    _context.BusinessLocationPsps.AddRange(psps);
                }

                // 🔹 INSERT BANKS
                if (model.Banks?.Any() == true)
                {
                    var banks = model.Banks.Select(b => new BusinessLocationBankSettlement
                    {
                        Id = Guid.NewGuid(),
                        Business_Location_Id = businessLocation.Id,
                        Bank_Id = b.Bank_Id,
                        Merchant_Share = b.Merchant_Share,
                        Bank_Share = b.Bank_Share,
                        Tax_Value = b.Tax_Value,
                        Tax_On_Merchant = b.Tax_On_Merchant ? 1 : 0,
                        Create_Date = DateTime.UtcNow,
                        Published = true,
                        Create_User = Guid.Parse(superuser.Id)
                    });

                    _context.BusinessLocationBankSettlements.AddRange(banks);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // 🔹 RELOAD WITH RELATED DATA
                var savedLocation = await _context.BusinessLocation
                    .AsNoTracking()
                    .Include(x => x.Psps)
                    .Include(x => x.BankSettlements)
                    .FirstAsync(x => x.Id == businessLocation.Id);

                // 🔹 MAP TO RESPONSE DTO
                var response = new BusinessLocationResponseDto
                {
                    Id = savedLocation.Id,
                    BusinessId = savedLocation.Business_Id,
                    Name = savedLocation.Name,
                    Address = savedLocation.Address,
                    Country = savedLocation.Country,
                    State = savedLocation.State,
                    City = savedLocation.City,
                    PostalCode = savedLocation.Postal_Code,
                    Phone = savedLocation.Phone,
                    Mobile = savedLocation.Mobile,
                    Email = savedLocation.Email,
                    Website = savedLocation.Website,

                    DiscountBeforeTax = savedLocation.DiscountBeforeTax,
                    PosCharge = savedLocation.PosCharge,
                    DiscountBeforePosCharge = savedLocation.DiscountBeforePosCharge,
                    ServiceCharges = savedLocation.ServiceCharges,
                    DiscountBeforeServiceCharge = savedLocation.DiscountBeforeServiceCharge,

                    LocationPsps = savedLocation.Psps.Select(p => new PspDto { 
                      Id = p.Id,
                      PspId = p.Id,
                    
                    }).ToList(),

                    LocationBanks = savedLocation.BankSettlements.Select(b => new BankSettlementDto
                    {
                        Id = b.Id,
                        Bank_Id = b.Bank_Id,
                        Merchant_Share = b.Merchant_Share,
                        Bank_Share = b.Bank_Share,
                        Tax_Value = b.Tax_Value,
                        TaxOnMerchant = b.Tax_On_Merchant
                    }).ToList()
                };

                return Ok(new
                {
                    Message = "Business Location Registered Successfully",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred.",
                    Detail = ex.Message
                });
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
                            join bs in _context.Business on b.Business_Id equals bs.Id into bsJoin
                            from bs in bsJoin.DefaultIfEmpty()
                            select new
                            {
                                b.Id,
                                b.Business_Id,
                                BusinessName = bs != null ? bs.Name : "Unknown",
                                b.Name,
                                b.Address,
                                b.Country,
                                b.State,
                                b.City,
                                b.Postal_Code,
                                b.Phone,
                                b.Mobile,
                                b.Email,
                                b.Website,
                                b.DiscountBeforeTax,
                                b.PosCharge,
                                b.DiscountBeforePosCharge,
                                b.ServiceCharges,
                                b.DiscountBeforeServiceCharge,
                                b.Create_Date,
                                b.Create_User,
                                b.Last_Update_Date,
                                b.Last_Update_User,
                                b.Published,
                                b.Deleted,
                            };

                // Step 2: Apply filtering
                if (!string.IsNullOrEmpty(filter.Name))
                    query = query.Where(q => q.Name.Contains(filter.Name));

                // Sort by CreateDate descending to get the latest profiles first
                query = query.OrderByDescending(q => q.Create_Date);

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
                    return Unauthorized(new { Message = "Authorization Token Is Missing Or Invalid." });

                // Step 6: Fetch external location names
                var apiService = new ApiServiceHelper(new HttpClient());

                var countryNames = await FetchLocationNameAsync(apiService, "MasterEntry", "countrie", countryIds, jwtToken);
                var stateNames = await FetchLocationNamesAsync(apiService, "State", "state", stateIds, jwtToken);
                var cityNames = await FetchLocationNamesAsync(apiService, "City", "citie", cityIds, jwtToken);

                // Step 7: Map to DTO list
                var result = businessLocations.Select(b => new BusinessLocationListModel
                {
                    Id = b.Id,
                    BusinessId = b.Business_Id,
                    BusinessName = b.BusinessName,
                    Name = b.Name,
                    Address = b.Address,
                    Country = countryNames.TryGetValue(b.Country, out var cName) ? cName : "Unknown",
                    State = stateNames.TryGetValue(b.State, out var sName) ? sName : "Unknown",
                    City = cityNames.TryGetValue(b.City, out var ctName) ? ctName : "Unknown",
                    PostalCode = b.Postal_Code,
                    Phone = b.Phone,
                    Mobile = b.Mobile,
                    Email = b.Email,
                    Website = b.Website,
                    DiscountBeforeTax = b.DiscountBeforeTax,
                    PosCharge = b.PosCharge,
                    DiscountBeforePosCharge = b.DiscountBeforePosCharge,
                    ServiceCharges = b.ServiceCharges,
                    DiscountBeforeServiceCharge = b.DiscountBeforeServiceCharge,
                    Create_Date = b.Create_Date,
                    Create_User = b.Create_User,
                    Last_Update_Date = b.Last_Update_Date,
                    Last_Update_User = b.Last_Update_User,
                    Published = b.Published,
                    Deleted = b.Deleted,
                }).OrderByDescending(b=> b.Create_Date).ToList();

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
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [Authorize]
        [HttpGet("get-location-bank-details")]
        public async Task<IActionResult> GetLocationBankDetails(Guid locationId)
        {
            if (locationId == Guid.Empty)
                return BadRequest(new { Message = "Location Id Is Required." });

            // 1️⃣ Get settlements
            var settlements = await _context.BusinessLocationBankSettlements
                .AsNoTracking()
                .Where(x => x.Business_Location_Id == locationId && !x.Deleted)
                .ToListAsync();

            if (!settlements.Any())
                return NoContent();

            // 2️⃣ JWT token
            var jwtToken = HttpContext.Request.Headers["Authorization"]
                .FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(jwtToken))
                return Unauthorized(new { Message = "Authorization Token Missing." });

            // 3️⃣ Prepare API helper
            var apiService = new ApiServiceHelper(new HttpClient());
            var baseUrl = _configuration["GlobalConfiguration:BaseUrl"];

            var result = new List<LocationBankDetailDto>();

            // 4️⃣ Call Bank Service PER BANK
            foreach (var settlement in settlements)
            {
                var bankUrl = $"{baseUrl}/apigateway/AdminService/MasterEntry/get-bank-by-id?id={settlement.Bank_Id}";

                var bankResponse = await apiService.SendRequestAsync<object>(
                    bankUrl,
                    HttpMethod.Get,
                    null,
                    jwtToken
                );

                string bankName = "Unknown";

                if (bankResponse != null)
                {
                    var root = bankResponse.Value;

                    if (root.TryGetProperty("bank", out var bankElement) &&
                        bankElement.TryGetProperty("name", out var nameProp))
                    {
                        bankName = nameProp.GetString() ?? "Unknown";
                    }
                }

                result.Add(new LocationBankDetailDto
                {
                    BankId = settlement.Bank_Id,
                    BankName = bankName,
                    MerchantShare = settlement.Merchant_Share,
                    BankShare = settlement.Bank_Share,
                    TaxValue = settlement.Tax_Value,
                    TaxOnMerchant = settlement.Tax_On_Merchant == 1
                });
            }

            return Ok(new
            {
                LocationId = locationId,
                Banks = result
            });
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
                    return BadRequest(new { Message = "Invalid Business Id." });

                // Get the count of locations for the given businessId
                int locationCount = await _businessLocationRepository.GetLocationCountByBusinessIdAsync(Guid.Parse(businessLocationCountModel.BusinessId));
                if (locationCount > 0)
                {
                    // Return the location count
                    return Ok(new { Business_Id = Guid.Parse(businessLocationCountModel.BusinessId), LocationCount = locationCount });
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
                    BusinessId = loc.Business_Id,
                    BusinessName = loc.Business?.Name ?? string.Empty,
                    Name = loc.Name,
                    Address = loc.Address,
                    Country = loc.Country,
                    State = loc.State,
                    City = loc.City,
                    PostalCode = loc.Postal_Code,
                    Phone = loc.Phone,
                    Mobile = loc.Mobile,
                    Email = loc.Email,
                    Website = loc.Website,
                    DiscountBeforeTax = loc.DiscountBeforeTax,
                    PosCharge = loc.PosCharge,
                    DiscountBeforePosCharge = loc.DiscountBeforePosCharge,
                    ServiceCharges = loc.ServiceCharges,
                    DiscountBeforeServiceCharge = loc.DiscountBeforeServiceCharge,
                    Deleted = loc.Deleted,
                    Published = loc.Published,
                    CreateDate = loc.Create_Date,
                    CreateUser = loc.Create_User,
                    LastUpdateDate = loc.Last_Update_Date,
                    LastUpdateUser = loc.Last_Update_User,
                }).OrderByDescending(dto => dto.CreateDate)
                .ToList();

                // wrap the list however you want; here I’m keeping the same wrapper key
                return Ok(new { BusinessLocations = dtos });
            }
            catch (Exception ex)
            {
                // log ex as usual
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [Authorize]
        [HttpGet("get-business-location-by-business-id")]
        public async Task<IActionResult> GetBusinessLocationsByBusinessId(Guid businessId)
        {
            try
            {
                if (businessId == Guid.Empty)
                    return BadRequest(new { Message = "Business Id Is Required." });

                var locations = await _context.BusinessLocation
                    .Include(x => x.Psps)
                    .Include(x => x.BankSettlements)
    .Where(x => x.Business_Id == businessId && !x.Deleted)
                    .OrderByDescending(x => x.Create_Date)
                    .ToListAsync();

                if (!locations.Any())
                    return NoContent();

                // Collect IDs for lookup
                var countryIds = locations.Select(x => x.Country).Distinct().ToList();
                var stateIds = locations.Select(x => x.State).Distinct().ToList();
                var cityIds = locations.Select(x => x.City).Distinct().ToList();

                // Token
                var jwtToken = HttpContext.Request.Headers["Authorization"]
                    .FirstOrDefault()?.Split(" ").Last();

                if (string.IsNullOrEmpty(jwtToken))
                    return Unauthorized(new { Message = "Authorization Token Missing." });

                var apiService = new ApiServiceHelper(new HttpClient());

                var countryNames = await FetchLocationNameAsync(apiService, "MasterEntry", "countrie", countryIds, jwtToken);
                var stateNames = await FetchLocationNamesAsync(apiService, "State", "state", stateIds, jwtToken);
                var cityNames = await FetchLocationNamesAsync(apiService, "City", "citie", cityIds, jwtToken);

                var result = locations.Select(x => new BusinessLocationsDto
                {
                    Id = x.Id,
                    BusinessId = x.Business_Id,
                    BusinessName = x.Business?.Name ?? string.Empty,
                    Name = x.Name,
                    Address = x.Address,
                    Country = x.Country,
                    CountryName = countryNames.TryGetValue(x.Country, out var cName) ? cName : "Unknown",
                    State = x.State,
                    StateName = stateNames.TryGetValue(x.State, out var sName) ? sName : "Unknown",
                    City = x.City,
                    CityName = cityNames.TryGetValue(x.City, out var ctName) ? ctName : "Unknown",
                    PostalCode = x.Postal_Code,
                    Phone = x.Phone,
                    Mobile = x.Mobile,
                    Email = x.Email,
                    Website = x.Website,
                    DiscountBeforeTax = x.DiscountBeforeTax,
                    PosCharge = x.PosCharge,
                    DiscountBeforePosCharge = x.DiscountBeforePosCharge,
                    ServiceCharges = x.ServiceCharges,
                    DiscountBeforeServiceCharge = x.DiscountBeforeServiceCharge,
                    Published = x.Published,
                    Deleted = x.Deleted,
                    CreateDate = x.Create_Date,
                    CreateUser = x.Create_User,
                    LastUpdateDate = x.Last_Update_Date,
                    LastUpdateUser = x.Last_Update_User,
                    LocationPsps = x.Psps.Select(p => new PspDto
                    {
                        Id = p.Id,
                        PspId = p.Psp_Id,
                        Published = p.Published,
                        Deleted = p.Deleted,
                        Create_Date = p.Create_Date,
                        Create_User = p.Create_User,
                        Last_Update_Date = p.Last_Update_Date,
                        Last_Update_User = p.Last_Update_User,
                    }).ToList(),

                    LocationBanks = x.BankSettlements.Select(b => new BankSettlementDto
                    {
                        Id = b.Id,
                        Bank_Id = b.Bank_Id,
                        Merchant_Share = b.Merchant_Share,
                        Bank_Share = b.Bank_Share,
                        Tax_Value = b.Tax_Value,
                        TaxOnMerchant = b.Tax_On_Merchant
                    }).ToList()


                }).ToList();

                return Ok(new { BusinessLocations = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
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
                    return NotFound("Business Location Not Found.");

                var dto = new BusinessLocationsDto
                {
                    Id = location.Id,
                    BusinessId = location.Business_Id,
                    BusinessName = location.Business?.Name ?? string.Empty,
                    Name = location.Name,
                    Address = location.Address,
                    Country = location.Country,
                    State = location.State,
                    City = location.City,
                    PostalCode = location.Postal_Code,
                    Mobile = location.Mobile,
                    Phone = location.Phone,
                    Email = location.Email,
                    Website = location.Website,
                    DiscountBeforeTax = location.DiscountBeforeTax,
                    PosCharge = location.PosCharge,
                    DiscountBeforePosCharge = location.DiscountBeforePosCharge,
                    ServiceCharges = location.ServiceCharges,
                    DiscountBeforeServiceCharge = location.DiscountBeforeServiceCharge,
                };


                return Ok(new { BusinessLocation = dto });
            }
            catch (Exception ex)
            {
                // log the exception here if you aren’t already
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPut("edit-business-location")]
        public async Task<IActionResult> EditBusinessLocation([FromBody] UpdateBusinessLocationModel updateDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Validate Id
                if (string.IsNullOrEmpty(updateDto.Id))
                    return BadRequest(new { Message = "Business Location Id Is Required." });

                if (string.IsNullOrEmpty(updateDto.BusinessId))
                    return BadRequest(new { Message = "Business Id Is Required." });

                // Validate Name
                if (string.IsNullOrEmpty(updateDto.Name))
                    return BadRequest(new { Message = "Business Location Name Is Required." });

                // Check if the business exists
                var businesslocation = await _businessLocationRepository.GetByIdAsync(Guid.Parse(updateDto.Id));
                if (businesslocation == null)
                    return NotFound(new { Message = "Business Location Not Found!." });

                // Check if User.Identity is null
                if (User?.Identity?.Name == null)
                    return Unauthorized(new { Message = "User Identity Is Not Available." });

                // Check if user exists
                var userName = User.Identity.Name;
                if (string.IsNullOrEmpty(userName))
                    return Unauthorized(new { Message = "User Name Is Not Available." });

                var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
                if (superuser == null) return Unauthorized("User Not Found.");

                // 🔥 UNIQUE NAME CHECK (EXCEPT CURRENT RECORD)
                var normalizedName = updateDto.Name.Trim().ToUpper();
                var businessId = Guid.Parse(updateDto.BusinessId);

                var duplicate = await _context.BusinessLocation
                    .Where(x =>
                        x.Business_Id == businessId &&
                        x.Id != businesslocation.Id &&                // Exclude current
                        x.Name.Trim().ToUpper() == normalizedName &&
                        !x.Deleted)
                    .FirstOrDefaultAsync();

                if (duplicate != null)
                {
                    return BadRequest(new { Message = "Another Business Location With This Name Already Exists For This Business." });
                }


                // Update the business properties
                businesslocation.Name = updateDto.Name.Trim();
                businesslocation.Business_Id = Guid.Parse(updateDto.BusinessId);
                businesslocation.Address = !string.IsNullOrEmpty(updateDto.Address) ? updateDto.Address : businesslocation.Address;
                businesslocation.Country = !string.IsNullOrEmpty(updateDto.Country) ? Guid.Parse(updateDto.Country) : businesslocation.Country;
                businesslocation.State = string.IsNullOrWhiteSpace(updateDto.State) ? businesslocation.State : Guid.Parse(updateDto.State);
                businesslocation.City = !string.IsNullOrEmpty(updateDto.City) ? Guid.Parse(updateDto.City) : businesslocation.City;
                businesslocation.Postal_Code = !string.IsNullOrEmpty(updateDto.PostalCode) ? updateDto.PostalCode : businesslocation.Postal_Code;
                businesslocation.Phone = !string.IsNullOrEmpty(updateDto.Phone) ? updateDto.Phone : businesslocation.Phone;
                businesslocation.Mobile = !string.IsNullOrEmpty(updateDto.Mobile) ? updateDto.Mobile : businesslocation.Mobile;
                businesslocation.Email = !string.IsNullOrEmpty(updateDto.Email) ? updateDto.Email : businesslocation.Email;
                businesslocation.Website = !string.IsNullOrEmpty(updateDto.Website) ? updateDto.Website : businesslocation.Website;
                businesslocation.DiscountBeforeTax = updateDto.DiscountBeforeTax;
                businesslocation.PosCharge = updateDto.PosCharge;
                businesslocation.DiscountBeforePosCharge = updateDto.DiscountBeforePosCharge;
                businesslocation.ServiceCharges = updateDto.ServiceCharges;
                businesslocation.DiscountBeforeServiceCharge = updateDto.DiscountBeforeServiceCharge;
                businesslocation.Last_Update_Date = DateTime.UtcNow;
                businesslocation.Published = true;
                businesslocation.Last_Update_User = Guid.Parse(superuser.Id);

                // Save updates
                _businessLocationRepository.Update(businesslocation);
                await _businessLocationRepository.SaveChangesAsync();

                var existingPsps = await _context.BusinessLocationPsps
                .Where(x => x.Business_Location_Id == businesslocation.Id)
                .ToListAsync();

                _context.BusinessLocationPsps.RemoveRange(existingPsps);

                if (updateDto.Psp_Ids != null && updateDto.Psp_Ids.Any())
                {
                    var newPsps = updateDto.Psp_Ids.Select(pspId => new BusinessLocationPsp
                    {
                        Id = Guid.NewGuid(),
                        Business_Location_Id = businesslocation.Id,
                        Psp_Id = pspId
                    });

                    _context.BusinessLocationPsps.AddRange(newPsps);
                }

                var existingBanks = await _context.BusinessLocationBankSettlements
                    .Where(x => x.Business_Location_Id == businesslocation.Id)
                    .ToListAsync();

                _context.BusinessLocationBankSettlements.RemoveRange(existingBanks);

                if (updateDto.Banks != null && updateDto.Banks.Any())
                {
                    var newBanks = updateDto.Banks.Select(b => new BusinessLocationBankSettlement
                    {
                        Id = Guid.NewGuid(),
                        Business_Location_Id = businesslocation.Id,
                        Bank_Id = b.Bank_Id,
                        Merchant_Share = b.Merchant_Share,
                        Bank_Share = b.Bank_Share,
                        Tax_Value = b.Tax_Value,
                        Tax_On_Merchant = b.Tax_On_Merchant ? 1 : 0
                    });

                    _context.BusinessLocationBankSettlements.AddRange(newBanks);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Return success response
                return Ok(new
                {
                    Message = "Business Location Updated Successfully!",
                    BusinessLocation = businesslocation
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Handle server errors
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = $"Error: {ex.Message}" });
            }
        }

        [Authorize]
        [HttpDelete("delete-business-location")]
        public async Task<IActionResult> DeleteBusinessLocation(DeleteBusinessLocationModel deleteBusinessLocation)
        {
            var businesslocation = await _businessLocationRepository.GetByIdAsync(Guid.Parse(deleteBusinessLocation.Id));
            if (businesslocation == null) return NotFound("Business Location Not Found.");

            // Check if User.Identity is null
            if (User?.Identity?.Name == null)
                return Unauthorized(new { Message = "User Identity Is Not Available." });

            // Check if user exists
            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(new { Message = "User name Is Not Available." });

            var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (superuser == null) return Unauthorized("User Not Found.");

            businesslocation.Deleted = true;
            businesslocation.Published = false;
            businesslocation.Last_Update_Date = DateTime.UtcNow;
            businesslocation.Last_Update_User = Guid.Parse(superuser.Id);

            _businessLocationRepository.Update(businesslocation);
            await _businessLocationRepository.SaveChangesAsync();

            return Ok(new { Message = "Business Location Marked As Deleted", BusinessLocation = businesslocation });
        }
        #endregion

        #region Function
        private async Task<Dictionary<Guid, string>> FetchLocationNamesAsync(ApiServiceHelper apiService, string locationType, string actiontype,List<Guid> ids, string jwtToken)
        {
            if (ids == null || !ids.Any())
                return new Dictionary<Guid, string>();

            var BaseUrl = _configuration["GlobalConfiguration:BaseUrl"];

            var apiUrl = $"{BaseUrl}/apigateway/SetupService/{locationType}/get-{actiontype.ToLower()}s";

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

        private async Task<Dictionary<Guid, string>> FetchLocationNameAsync(ApiServiceHelper apiService, string locationType, string actiontype, List<Guid> ids, string jwtToken)
        {
            if (ids == null || !ids.Any())
                return new Dictionary<Guid, string>();

            var BaseUrl = _configuration["GlobalConfiguration:BaseUrl"];

            var apiUrl = $"{BaseUrl}/apigateway/AdminService/{locationType}/get-{actiontype.ToLower()}s";

            var response = await apiService.SendRequestAsync<object, List<Dictionary<string, object>>>(
    apiUrl,
    HttpMethod.Get,
    null,
    jwtToken
) ?? new List<Dictionary<string, object>>();

            return response
                .Where(c => c.ContainsKey("id") && c.ContainsKey("name"))
                .ToDictionary(
                    c => Guid.Parse(c["id"]!.ToString()!),
                    c => c["name"]!.ToString()!
                );

        }

        #endregion
    }
}
