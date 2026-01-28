using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NanoDMSBusinessService.Data;
using NanoDMSBusinessService.DTO;
using NanoDMSBusinessService.Models;
using NanoDMSBusinessService.Repositories;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace NanoDMSBusinessService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IBusinessRepository _businessRepository;

        #region Constructor
        public BusinessController(AppDbContext context,
            IBusinessRepository businessRepository,
            IConfiguration configuration,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _businessRepository = businessRepository;
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;

        }
        #endregion

        #region Apis

        [Authorize]
        [HttpPost("register-business")]
        public async Task<IActionResult> RegisterBusiness([FromForm] RegisterBusinessModel model)
        {
            try
            {
                // Validate Business Name
                if (string.IsNullOrEmpty(model.Name))
                    return BadRequest(new { Message = "Business Name Is Required." });

                // Validate StartDate
                if (model.StartDate == default || model.StartDate > DateTime.UtcNow)
                    return BadRequest(new { Message = "Invalid Start Date." });

                // Validate TimeZoneId
                if (model.TimeZoneId == Guid.Empty)
                    return BadRequest(new { Message = "Time Zone Is Required." });

                // Validate CurrencyId
                if (model.CurrencyId == Guid.Empty)
                    return BadRequest(new { Message = "Currency Is Required." });

                // Validate FinancialYearStartMonth
                if (model.FinancialYearStartMonth == Guid.Empty)
                    return BadRequest(new { Message = "Financial Year Start Month Is Required." });

                // Validate StockAccountingMethod
                if (model.StockAccountingMethod == Guid.Empty)
                    return BadRequest(new { Message = "Stock Accounting Method Is Required." });

                // Check if User.Identity is null
                if (User?.Identity?.Name == null)
                    return Unauthorized(new { Message = "User identity Is Not Available." });

                // Check if user exists
                var userName = User.Identity.Name;
                if (string.IsNullOrEmpty(userName))
                    return Unauthorized(new { Message = "User name Is Not Available." });

                // Fix for CS8604: Ensure userName is not null before passing it to FindByNameAsync
                var superuser = await _userManager.FindByNameAsync(userName);
                if (superuser == null)
                    return Unauthorized(new { Message = "User Not Found." });

                // Map DTO to entity (Assuming there's a Business entity)
                var business = new Business
                {
                    Name = model.Name,
                    Start_Date = model.StartDate,
                    Time_Zone_Id = model.TimeZoneId,
                    Currency_Id = model.CurrencyId,
                    Financial_Year_Start_Month = model.FinancialYearStartMonth,
                    Stock_Accounting_Method = model.StockAccountingMethod,
                    Ntn = model.Ntn,
                    Stn = model.Stn,
                    Tax3 = model.Tax3,
                    Tax4 = model.Tax4,
                    Create_Date = DateTime.UtcNow,
                    Published = true,
                    Create_User = Guid.Parse(superuser.Id)
                };

                // Save to repository
                await _businessRepository.AddAsync(business);
                await _businessRepository.SaveChangesAsync();

                // Process and save the profile image
                if (model.Logo != null && model.Logo.Length > 0)
                {
                    // Define the root directory for images
                    var imagesRootPath = _configuration["ImageSettings:ImagesRootPath"];

                    if (string.IsNullOrWhiteSpace(imagesRootPath))
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, "Image root path configuration is missing.");
                    }
                    // Define subfolder dynamically (e.g., UserProfile, ProductImages, etc.)
                    var subFolder = "BusinessProfile"; // Change this dynamically based on use case
                    var staticPath = Path.Combine(imagesRootPath, subFolder);

                    if (!Directory.Exists(staticPath))
                    {
                        Directory.CreateDirectory(staticPath);
                    }

                    // Rename the image file using UserId
                    var fileExtension = Path.GetExtension(model.Logo.FileName);
                    var newFileName = $"{business.Id}{fileExtension}";
                    var filePath = Path.Combine(staticPath, newFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Logo.CopyToAsync(stream);
                    }

                    // Save relative path instead of absolute path
                    business.Logo = $"/Images/{subFolder}/{newFileName}";

                    _businessRepository.Update(business);
                    await _businessRepository.SaveChangesAsync();
                }

                // Return success response
                return Ok(new { Message = "Business Registered Successfully", Business = business });
            }
            catch (Exception ex)
            {
                // Handle server errors
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = $"Error: {ex.Message}" });
            }
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-business-list")]
        public async Task<IActionResult> GetBusinessList([FromQuery] BusinessFilterModel filter)
        {
            try
            {
                var query = _context.Business.Select(b => new Business
                {
                    Id = b.Id,
                    Name = b.Name,
                    Start_Date = b.Start_Date,
                    Time_Zone_Id = b.Time_Zone_Id,
                    Currency_Id = b.Currency_Id,
                    Financial_Year_Start_Month = b.Financial_Year_Start_Month,
                    Stock_Accounting_Method = b.Stock_Accounting_Method,
                    Logo = b.Logo,
                    Ntn = b.Ntn,
                    Stn = b.Stn,
                    Tax3 = b.Tax3,
                    Tax4 = b.Tax4,
                    Create_Date = b.Create_Date,
                    Create_User = b.Create_User,
                    Last_Update_Date = b.Last_Update_Date,
                    Last_Update_User = b.Last_Update_User,
                    Published = b.Published,
                    Deleted = b.Deleted,
                }).AsQueryable();



                if (!string.IsNullOrEmpty(filter.Name))
                    query = query.Where(q => q.Name.Contains(filter.Name));

                // Sort by CreateDate descending to get the latest profiles first
                query = query.OrderByDescending(q => q.Create_Date);

                // **Apply Pagination**
                var totalRecords = await query.CountAsync();
                // Apply pagination
                var businesses = await query
                    .Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToListAsync();

                if (!businesses.Any())
                {
                    return NoContent();
                }

                var response = new PaginatedResponseDto<Business>
                {
                    TotalRecords = totalRecords,
                    PageNumber = filter.PageNumber,
                    PageSize = filter.PageSize,
                    TotalPages = (int)Math.Ceiling((double)totalRecords / filter.PageSize),
                    Data = businesses
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
        [HttpGet("get-businesses")]
        public async Task<IActionResult> GetBusinesses()
        {
            try
            {
                var businesses = await _context.Business.OrderByDescending(b=> b.Create_Date).ToListAsync();
                if (!businesses.Any()) return NotFound("No Businesses Found!.");

                return Ok(new { Business = businesses });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpPost("get-business-by-id")]
        public async Task<IActionResult> GetBusinessById(BusinessByIdModel businessById)
        {
            try
            {
                var business = await _context.Business.FindAsync(Guid.Parse(businessById.Id));

                return business == null ? NotFound("Business Not Found.") : Ok(new { Business = business });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPut("edit-business")]
        public async Task<IActionResult> EditBusiness([FromForm] UpdateBusinessModel updateDto)
        {
            try
            {
                // Validate Id
                if (string.IsNullOrEmpty(updateDto.Id))
                    return BadRequest(new { Message = "Business Id Is Required." });

                // Validate Name
                if (string.IsNullOrEmpty(updateDto.Name))
                    return BadRequest(new { Message = "Business Name Is Required." });

                // Check if the business exists
                var business = await _businessRepository.GetByIdAsync(Guid.Parse(updateDto.Id));
                if (business == null)
                    return NotFound(new { Message = "Business Not Found!." });

                // Check if User.Identity is null
                if (User?.Identity?.Name == null)
                    return Unauthorized(new { Message = "User Identity Is Not Available." });

                // Check if user exists
                var userName = User.Identity.Name;
                if (string.IsNullOrEmpty(userName))
                    return Unauthorized(new { Message = "User Name Is Not Available." });

                var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
                if (superuser == null) return Unauthorized("User Not Found.");

                // Handle image update if a new image is provided
                if (updateDto.Logo != null && updateDto.Logo.Length > 0)
                {
                    // Define the root directory for images
                    var imagesRootPath = _configuration["ImageSettings:ImagesRootPath"];

                    if (string.IsNullOrWhiteSpace(imagesRootPath))
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, "Image root path configuration is missing.");
                    }

                    // Define subfolder dynamically (e.g., UserProfile, ProductImages, etc.)
                    var subFolder = "BusinessProfile"; // Change this dynamically based on use case
                    var staticPath = Path.Combine(imagesRootPath, subFolder);

                    if (!Directory.Exists(staticPath))
                    {
                        Directory.CreateDirectory(staticPath);
                    }

                    // Rename the image file using the UserId
                    var fileExtension = Path.GetExtension(updateDto.Logo.FileName);
                    var newFileName = $"{business.Id}{fileExtension}";
                    var filePath = Path.Combine(staticPath, newFileName);

                    // Delete the old image if it exists
                    if (!string.IsNullOrEmpty(business.Logo))
                    {
                        var oldImagePath = business.Logo;

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await updateDto.Logo.CopyToAsync(stream);
                    }

                    // Save the static path in the database (use relative or absolute path based on requirement)
                    business.Logo = $"/Images/{subFolder}/{newFileName}"; // Save absolute file path


                }

                // Update the business properties
                business.Name = updateDto.Name;
                business.Start_Date = updateDto.StartDate != default ? updateDto.StartDate : business.Start_Date;
                business.Ntn = !string.IsNullOrEmpty(updateDto.Ntn) ? updateDto.Ntn : business.Ntn;
                business.Stn = !string.IsNullOrEmpty(updateDto.Stn) ? updateDto.Stn : business.Stn;
                business.Tax3 = !string.IsNullOrEmpty(updateDto.Tax3) ? updateDto.Tax3 : business.Tax3;
                business.Tax4 = !string.IsNullOrEmpty(updateDto.Tax4) ? updateDto.Tax4 : business.Tax4;
                business.Last_Update_Date = DateTime.UtcNow;
                business.Published = true;
                business.Last_Update_User = Guid.Parse(superuser.Id);

                // Save updates
                _businessRepository.Update(business);
                await _businessRepository.SaveChangesAsync();

                // Return success response
                return Ok(new
                {
                    Message = "Business Updated Successfully!",
                    Business = business
                });
            }
            catch (Exception ex)
            {
                // Handle server errors
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = $"Error: {ex.Message}" });
            }
        }

        [Authorize]
        [HttpDelete("delete-business")]
        public async Task<IActionResult> DeleteBusiness(DeleteBusinessModel deleteBusiness)
        {
            var business = await _businessRepository.GetByIdAsync(Guid.Parse(deleteBusiness.Id));
            if (business == null) return NotFound("Business Not Found.");

            // Check if User.Identity is null
            if (User?.Identity?.Name == null)
                return Unauthorized(new { Message = "User Identity Is Not Available." });

            // Check if user exists
            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(new { Message = "User Name Is Not Available." });

            var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (superuser == null) return Unauthorized("User Not Found.");

            business.Deleted = true;
            business.Published = false;
            business.Last_Update_Date = DateTime.UtcNow;
            business.Last_Update_User = Guid.Parse(superuser.Id);

            _businessRepository.Update(business);
            await _businessRepository.SaveChangesAsync();

            return Ok(new { Message = "Business Marked As Deleted", Business = business });
        }

        #endregion






    }
}
