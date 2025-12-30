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
    public class CityController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ICityRepository _cityRepository;

        #region Constructor
        public CityController(AppDbContext context,
           ICityRepository cityRepository,
           IConfiguration configuration,
           UserManager<IdentityUser> userManager,
           RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
            _cityRepository = cityRepository;

        }
        #endregion

        #region Apis

        [Authorize]
        [HttpPost("register-city")]
        public async Task<IActionResult> RegisterCity([FromBody] RegisterCityModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Name))
                    return BadRequest(new { Message = "City Name is required." });

                if (string.IsNullOrEmpty(model.StateId))
                    return BadRequest(new { Message = "State Id is required." });

                // Check if User.Identity is null
                if (User?.Identity?.Name == null)
                    return Unauthorized(new { Message = "User identity is not available." });

                // Check if user exists
                var userName = User.Identity.Name;
                if (string.IsNullOrEmpty(userName))
                    return Unauthorized(new { Message = "User name is not available." });


                var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
                if (superuser == null) return Unauthorized("User not found.");

                var city = new City
                {
                    Name = model.Name,
                    State_Id = Guid.Parse(model.StateId),
                    Create_Date = DateTime.UtcNow,
                    Published = true,
                    Create_User = Guid.Parse(superuser.Id)
                };

                await _cityRepository.AddAsync(city);
                await _cityRepository.SaveChangesAsync();

                return Ok(new { Message = "City registered successfully", City = city });

            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }

        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-cities")]
        public async Task<IActionResult> GetCities()
        {
            try
            {
                var cities = await _cityRepository.GetAllAsync();

                if (!cities.Any()) return NotFound("No Cities found.");

                return Ok(new { City = cities });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpPost("get-city-by-id")]
        public async Task<IActionResult> GetCityById(CityByIdModel cityById)
        {
            try
            {
                var city = await _cityRepository.GetByIdAsync(Guid.Parse(cityById.Id));

                return city == null ? NotFound("City not found.") : Ok(new { City = city });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPut("edit-city")]
        public async Task<IActionResult> EditCity([FromBody] UpdateCityModel updateDto)
        {
            if (string.IsNullOrEmpty(updateDto.Name))
            {
                return BadRequest(new { Message = "City Name is required." });
            }
            var city = await _cityRepository.GetByIdAsync(Guid.Parse(updateDto.Id));
            if (city == null) return NotFound("City not found.");

            // Check if User.Identity is null
            if (User?.Identity?.Name == null)
                return Unauthorized(new { Message = "User identity is not available." });

            // Check if user exists
            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(new { Message = "User name is not available." });


            var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (superuser == null) return Unauthorized("User not found.");

            city.Name = updateDto.Name;
            city.Last_Update_Date = DateTime.UtcNow;
            city.Published = true;
            city.Last_Update_User = Guid.Parse(superuser.Id);

            _cityRepository.Update(city);
            await _cityRepository.SaveChangesAsync();

            return Ok(new { Message = "City updated successfully", City = city });
        }

        [Authorize]
        [HttpDelete("delete-city")]
        public async Task<IActionResult> DeleteCity(DeleteCityModel deleteCity)
        {
            var city = await _cityRepository.GetByIdAsync(Guid.Parse(deleteCity.Id));
            if (city == null) return NotFound("City not found.");

            // Check if User.Identity is null
            if (User?.Identity?.Name == null)
                return Unauthorized(new { Message = "User identity is not available." });

            // Check if user exists
            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(new { Message = "User name is not available." });


            var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (superuser == null) return Unauthorized("User not found.");

            city.Deleted = true;
            city.Published = false;
            city.Last_Update_Date = DateTime.UtcNow;
            city.Last_Update_User = Guid.Parse(superuser.Id);

            _cityRepository.Update(city);
            await _cityRepository.SaveChangesAsync();

            return Ok(new { Message = "City marked as Deleted", City = city });
        }

        #endregion

    }
}
