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
    public class TimeZoneController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;       
        private readonly ITimeZoneRepository _timeZoneRepository;

        #region Constructor
        public TimeZoneController(AppDbContext context,
            ITimeZoneRepository timeZoneRepository,
            IConfiguration configuration,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _timeZoneRepository = timeZoneRepository;
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;

        }
        #endregion

        #region Apis

        [Authorize]
        [HttpPost("register-time-zone")]
        public async Task<IActionResult> RegisterTimeZone([FromBody] RegisterTimeZoneModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Name))
                    return BadRequest(new { Message = "Time Zone Name is required." });

                if (string.IsNullOrEmpty(model.GMTSetting))
                    return BadRequest(new { Message = "GMTSetting is required." });

                // Check if User.Identity is null
                if (User?.Identity?.Name == null)
                    return Unauthorized(new { Message = "User identity is not available." });

                // Check if user exists
                var userName = User.Identity.Name;
                if (string.IsNullOrEmpty(userName))
                    return Unauthorized(new { Message = "User name is not available." });


                var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
                if (superuser == null) return Unauthorized("User not found.");

                var timeZone = new Models.TimeZone
                {
                    Name = model.Name,
                    GMTSetting = model.GMTSetting,
                    CreateDate = DateTime.UtcNow,
                    Published = true,
                    CreateUser = Guid.Parse(superuser.Id)
                };

                await _timeZoneRepository.AddAsync(timeZone);
                await _timeZoneRepository.SaveChangesAsync();

                return Ok(new { Message = "Time Zone registered successfully", TimeZone = timeZone });

            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }

        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-time-zones")]
        public async Task<IActionResult> GetTimeZones()
        {
            try
            {
                var timeZones = await _timeZoneRepository.GetAllAsync();

                if (!timeZones.Any()) return NotFound("No Time Zones found.");

                return Ok(new { TimeZone = timeZones });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpPost("get-time-zone-by-id")]
        public async Task<IActionResult> GetTimeZoneById(TimeZoneByIdModel timeZoneById)
        {
            try
            {
                var timezone = await _timeZoneRepository.GetByIdAsync(Guid.Parse(timeZoneById.Id));

                return timezone == null ? NotFound("Time Zone not found.") : Ok(new { TimeZone = timezone });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPut("edit-time-zone")]
        public async Task<IActionResult> EditTimeZone([FromBody] UpdateTimeZoneModel updateDto)
        {
            if (string.IsNullOrEmpty(updateDto.Name))
            {
                return BadRequest(new { Message = "Time Zone Name is required." });
            }

            if (string.IsNullOrEmpty(updateDto.GMTSetting))
            {
                return BadRequest(new { Message = "GMTSetting  is required." });
            }

            var timezone = await _timeZoneRepository.GetByIdAsync(Guid.Parse(updateDto.Id));
            if (timezone == null) return NotFound("TimeZone not found.");

            // Check if User.Identity is null
            if (User?.Identity?.Name == null)
                return Unauthorized(new { Message = "User identity is not available." });

            // Check if user exists
            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(new { Message = "User name is not available." });


            var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (superuser == null) return Unauthorized("User not found.");

            timezone.Name = updateDto.Name;
            timezone.GMTSetting = updateDto.GMTSetting;
            timezone.LastUpdateDate = DateTime.UtcNow;
            timezone.Published = true;
            timezone.LastUpdateUser = Guid.Parse(superuser.Id);

            _timeZoneRepository.Update(timezone);
            await _timeZoneRepository.SaveChangesAsync();

            return Ok(new { Message = "Time Zone updated successfully", TimeZone = timezone });
        }

        [Authorize]
        [HttpDelete("delete-time-zone")]
        public async Task<IActionResult> DeleteTimeZone(DeleteTimeZoneModel deleteTimeZoneModel)
        {
            var timezone = await _timeZoneRepository.GetByIdAsync(Guid.Parse(deleteTimeZoneModel.Id));
            if (timezone == null) return NotFound("Time Zone not found.");

            // Check if User.Identity is null
            if (User?.Identity?.Name == null)
                return Unauthorized(new { Message = "User identity is not available." });

            // Check if user exists
            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(new { Message = "User name is not available." });


            var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (superuser == null) return Unauthorized("User not found.");

            timezone.Deleted = true;
            timezone.Published = false;
            timezone.LastUpdateDate = DateTime.UtcNow;
            timezone.LastUpdateUser = Guid.Parse(superuser.Id);

            _timeZoneRepository.Update(timezone);
            await _timeZoneRepository.SaveChangesAsync();

            return Ok(new { Message = "Time Zone marked as Deleted", TimeZone = timezone });
        }

        #endregion
    }
}
