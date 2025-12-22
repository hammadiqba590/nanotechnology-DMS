using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NanoDMSSetupService.Data;
using NanoDMSSetupService.DTO;
using NanoDMSSetupService.Models;
using NanoDMSSetupService.Repositories;

namespace NanoDMSSetupService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinancialYearController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IFinancialYearRepository _financialYearRepository;

        #region Constructor
        public FinancialYearController(AppDbContext context,
            IFinancialYearRepository financialYearRepository,
            IConfiguration configuration,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _financialYearRepository = financialYearRepository;
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;

        }

        #endregion

        #region Apis

        [Authorize]
        [HttpPost("register-financial-year")]
        public async Task<IActionResult> RegisterFinancialYear([FromBody] RegisterFinancialYearModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Name))
                    return BadRequest(new { Message = "Financial Year Name is required." });

                // Check if User.Identity is null
                if (User?.Identity?.Name == null)
                    return Unauthorized(new { Message = "User identity is not available." });

                // Check if user exists
                var userName = User.Identity.Name;
                if (string.IsNullOrEmpty(userName))
                    return Unauthorized(new { Message = "User name is not available." });


                var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
                if (superuser == null) return Unauthorized("User not found.");

                var financialyear = new FinancialYearStartMonth
                {
                    Name = model.Name,
                    CreateDate = DateTime.UtcNow,
                    Published = true,
                    CreateUser = Guid.Parse(superuser.Id)
                };

                await _financialYearRepository.AddAsync(financialyear);
                await _financialYearRepository.SaveChangesAsync();

                return Ok(new { Message = "Financial Year registered successfully", FinancialYear = financialyear });

            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }

        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-financial-years")]
        public async Task<IActionResult> GetFinancialYears()
        {
            try
            {
                var financialyears = await _financialYearRepository.GetAllAsync();

                if (!financialyears.Any()) return NotFound("No Financial Years found.");

                return Ok(new { FinacialYear = financialyears });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpPost("get-financial-year-by-id")]
        public async Task<IActionResult> GetFinancialYearById(FinancialYearByIdModel financialYearByIdModel)
        {
            try
            {
                var financialyear = await _financialYearRepository.GetByIdAsync(Guid.Parse(financialYearByIdModel.Id));

                return financialyear == null ? NotFound("Financial Year not found.") : Ok(new { FinancialYear = financialyear });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPut("edit-financial-year")]
        public async Task<IActionResult> EditFinancialYear([FromBody] UpdateFinancialYearModel updateDto)
        {
            if (string.IsNullOrEmpty(updateDto.Name))
            {
                return BadRequest(new { Message = "Time Zone Name is required." });
            }

            var financialyear = await _financialYearRepository.GetByIdAsync(Guid.Parse(updateDto.Id));
            if (financialyear == null) return NotFound("Financial Year not found.");

            // Check if User.Identity is null
            if (User?.Identity?.Name == null)
                return Unauthorized(new { Message = "User identity is not available." });

            // Check if user exists
            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(new { Message = "User name is not available." });


            var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (superuser == null) return Unauthorized("User not found.");

            financialyear.Name = updateDto.Name;
            financialyear.LastUpdateDate = DateTime.UtcNow;
            financialyear.Published = true;
            financialyear.LastUpdateUser = Guid.Parse(superuser.Id);

            _financialYearRepository.Update(financialyear);
            await _financialYearRepository.SaveChangesAsync();

            return Ok(new { Message = "Financial Year updated successfully", FinancialYear = financialyear });
        }

        [Authorize]
        [HttpDelete("delete-financial-year")]
        public async Task<IActionResult> DeleteFinancialYear(DeleteFinancialYearModel deleteFinancialYearModel)
        {
            var financialyear = await _financialYearRepository.GetByIdAsync(Guid.Parse(deleteFinancialYearModel.Id));
            if (financialyear == null) return NotFound("Financial Year not found.");

            // Check if User.Identity is null
            if (User?.Identity?.Name == null)
                return Unauthorized(new { Message = "User identity is not available." });

            // Check if user exists
            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(new { Message = "User name is not available." });


            var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (superuser == null) return Unauthorized("User not found.");

            financialyear.Deleted = true;
            financialyear.Published = false;
            financialyear.LastUpdateDate = DateTime.UtcNow;
            financialyear.LastUpdateUser = Guid.Parse(superuser.Id);

            _financialYearRepository.Update(financialyear);
            await _financialYearRepository.SaveChangesAsync();

            return Ok(new { Message = "Financial Year marked as Deleted", FinancialYear = financialyear });
        }

        #endregion
    }
}
