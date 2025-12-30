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
    public class GenderController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IGenderRepository _genderRepository;

        #region Constructor
        public GenderController(AppDbContext context,
            IGenderRepository genderRepository,
            IConfiguration configuration,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _genderRepository = genderRepository;
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;

        }
        #endregion

        #region Apis

        [Authorize]
        [HttpPost("register-gender")]
        public async Task<IActionResult> RegisterGender([FromBody] RegisterGenderModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Name))
                    return BadRequest(new { Message = "Gender Name is required." });

                // Check if User.Identity is null
                if (User?.Identity?.Name == null)
                    return Unauthorized(new { Message = "User identity is not available." });

                // Check if user exists
                var userName = User.Identity.Name;
                if (string.IsNullOrEmpty(userName))
                    return Unauthorized(new { Message = "User name is not available." });


                var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
                if (superuser == null) return Unauthorized("User not found.");

                var gender = new Gender
                {
                    Name = model.Name,
                    Create_Date = DateTime.UtcNow,
                    Published = true,
                    Create_User = Guid.Parse(superuser.Id)
                };

                await _genderRepository.AddAsync(gender);
                await _genderRepository.SaveChangesAsync();

                return Ok(new { Message = "Gender registered successfully", Gender = gender });

            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }

        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-genders")]
        public async Task<IActionResult> GetGenders()
        {
            try
            {
                var genders = await _genderRepository.GetAllAsync();

                if (!genders.Any()) return NotFound("No Genders found.");

                return Ok(new { Gender = genders });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpPost("get-gender-by-id")]
        public async Task<IActionResult> GetGenderById(GenderByIDModel genderByID)
        {
            try
            {
                var gender = await _genderRepository.GetByIdAsync(Guid.Parse(genderByID.Id));

                return gender == null ? NotFound("Gender not found.") : Ok(new { Gender = gender });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPut("edit-gender")]
        public async Task<IActionResult> EditGender([FromBody] UpdateGenderModel updateDto)
        {
            if (string.IsNullOrEmpty(updateDto.Name))
            {
                return BadRequest(new { Message = "Gender Name is required." });
            }
            var gender = await _genderRepository.GetByIdAsync(Guid.Parse(updateDto.Id));
            if (gender == null) return NotFound("Gender not found.");

            // Check if User.Identity is null
            if (User?.Identity?.Name == null)
                return Unauthorized(new { Message = "User identity is not available." });

            // Check if user exists
            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(new { Message = "User name is not available." });


            var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (superuser == null) return Unauthorized("User not found.");

            gender.Name = updateDto.Name;
            gender.Last_Update_Date = DateTime.UtcNow;
            gender.Published = true;
            gender.Last_Update_User = Guid.Parse(superuser.Id);

            _genderRepository.Update(gender);
            await _genderRepository.SaveChangesAsync();

            return Ok(new { Message = "Gender updated successfully", Gender = gender });
        }

        [Authorize]
        [HttpDelete("delete-gender")]
        public async Task<IActionResult> DeleteGender(DeleteGenderModel deleteGender)
        {
            var gender = await _genderRepository.GetByIdAsync(Guid.Parse(deleteGender.Id));
            if (gender == null) return NotFound("Gender not found.");

            // Check if User.Identity is null
            if (User?.Identity?.Name == null)
                return Unauthorized(new { Message = "User identity is not available." });

            // Check if user exists
            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(new { Message = "User name is not available." });


            var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (superuser == null) return Unauthorized("User not found.");

            gender.Deleted = true;
            gender.Published = false;
            gender.Last_Update_Date = DateTime.UtcNow;
            gender.Last_Update_User = Guid.Parse(superuser.Id);

            _genderRepository.Update(gender);
            await _genderRepository.SaveChangesAsync();

            return Ok(new { Message = "Gender marked as Deleted", Gender = gender });
        }

        #endregion

    }
}
