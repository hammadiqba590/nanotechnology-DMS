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
    public class MaritalStatusController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMaritalRepository _maritalRepository;

        #region Constructor
        public MaritalStatusController(AppDbContext context,
           IMaritalRepository maritalRepository,
           IConfiguration configuration,
           UserManager<IdentityUser> userManager,
           RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _maritalRepository = maritalRepository;
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;

        }
        #endregion

        #region Apis

        [Authorize]
        [HttpPost("register-marital-status")]
        public async Task<IActionResult> RegisterMaritalStatus([FromBody] RegisterMaritalStatusModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Name))
                    return BadRequest(new { Message = "Marital Status Name is required." });

                // Check if User.Identity is null
                if (User?.Identity?.Name == null)
                    return Unauthorized(new { Message = "User identity is not available." });

                // Check if user exists
                var userName = User.Identity.Name;
                if (string.IsNullOrEmpty(userName))
                    return Unauthorized(new { Message = "User name is not available." });


                var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
                if (superuser == null) return Unauthorized("User not found.");

                var marital = new MaritalStatus
                {
                    Name = model.Name,
                    Create_Date = DateTime.UtcNow,
                    Published = true,
                    Create_User = Guid.Parse(superuser.Id)
                };

                await _maritalRepository.AddAsync(marital);
                await _maritalRepository.SaveChangesAsync();

                return Ok(new { Message = "Marital Status registered successfully", MaritalStatus = marital });

            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }

        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-marital-statuses")]
        public async Task<IActionResult> GetMaritalStatuses()
        {
            try
            {
                var maritals = await _maritalRepository.GetAllAsync();

                if (!maritals.Any()) return NotFound("No Marital Statuses found.");

                return Ok(new { MaritalStatus = maritals });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpPost("get-marital-status-by-id")]
        public async Task<IActionResult> GetMaritalStautsById(MaritalStatusById maritalStatusById)
        {
            try
            {
                var marital = await _maritalRepository.GetByIdAsync(Guid.Parse(maritalStatusById.Id));

                return marital == null ? NotFound("Marital Status not found.") : Ok(new { MaritalStatus = marital });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPut("edit-marital-status")]
        public async Task<IActionResult> EditMaritalStatus([FromBody] UpdateMaritalStatusModel updateDto)
        {
            if (string.IsNullOrEmpty(updateDto.Name))
            {
                return BadRequest(new { Message = "Marital Status Name is required." });
            }
            var marital = await _maritalRepository.GetByIdAsync(Guid.Parse(updateDto.Id));
            if (marital == null) return NotFound("Marital Status not found.");

            // Check if User.Identity is null
            if (User?.Identity?.Name == null)
                return Unauthorized(new { Message = "User identity is not available." });

            // Check if user exists
            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(new { Message = "User name is not available." });


            var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (superuser == null) return Unauthorized("User not found.");

            marital.Name = updateDto.Name;
            marital.Last_Update_Date = DateTime.UtcNow;
            marital.Published = true;
            marital.Last_Update_User = Guid.Parse(superuser.Id);

            _maritalRepository.Update(marital);
            await _maritalRepository.SaveChangesAsync();

            return Ok(new { Message = "Marital Status updated successfully", MaritalStatus = marital });
        }

        [Authorize]
        [HttpDelete("delete-marital-status")]
        public async Task<IActionResult> DeleteMaritalStatus(DeleteMaritalStatusModel deleteMaritalStatus)
        {
            var marital = await _maritalRepository.GetByIdAsync(Guid.Parse(deleteMaritalStatus.Id));
            if (marital == null) return NotFound("Marital Status not found.");

            // Check if User.Identity is null
            if (User?.Identity?.Name == null)
                return Unauthorized(new { Message = "User identity is not available." });

            // Check if user exists
            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(new { Message = "User name is not available." });


            var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (superuser == null) return Unauthorized("User not found.");

            marital.Deleted = true;
            marital.Published = false;
            marital.Last_Update_Date = DateTime.UtcNow;
            marital.Last_Update_User = Guid.Parse(superuser.Id);

            _maritalRepository.Update(marital);
            await _maritalRepository.SaveChangesAsync();

            return Ok(new { Message = "Marital Status marked as Deleted", MaritalStatus = marital });
        }

        #endregion
    }
}
