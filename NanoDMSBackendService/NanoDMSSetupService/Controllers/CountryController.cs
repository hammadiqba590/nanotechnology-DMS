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
    public class CountryController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ICountryRepository _countryRepository;

        #region Constructor
        public CountryController(AppDbContext context,
            ICountryRepository countryRepository,
            IConfiguration configuration,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _countryRepository = countryRepository;
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;

        }

        #endregion

        #region Apis

        [Authorize]
        [HttpPost("register-country")]
        public async Task<IActionResult> RegisterCountry([FromBody] RegisterCountryModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Name))
                    return BadRequest(new { Message = "Country Name is required." });

                // Check if User.Identity is null
                if (User?.Identity?.Name == null)
                    return Unauthorized(new { Message = "User identity is not available." });

                // Check if user exists
                var userName = User.Identity.Name;
                if (string.IsNullOrEmpty(userName))
                    return Unauthorized(new { Message = "User name is not available." });


                var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
                if (superuser == null) return Unauthorized("User not found.");

                var country = new Country
                {
                    Name = model.Name,
                    CreateDate = DateTime.UtcNow,
                    Published = true,
                    CreateUser = Guid.Parse(superuser.Id)
                };

                await _countryRepository.AddAsync(country);
                await _countryRepository.SaveChangesAsync();

                return Ok(new { Message = "Country registered successfully", Country = country });

            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }

        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-countries")]
        public async Task<IActionResult> GetCountries()
        {
            try
            {
                var countries = await _countryRepository.GetAllAsync();
                if (!countries.Any()) return NotFound("No countries found.");

                return Ok(new { Country = countries });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-country-details-by-id")]
        public async Task<IActionResult> GetCountryDetailsById([FromQuery] CountryByIdModel query)
        {
            try
            {
                if (Guid.Parse(query.Id) == Guid.Empty)
                    return BadRequest("Country ID is required.");

                var country = await _context.Country
                    .Where(c => c.Id == Guid.Parse(query.Id))
                    .Select(c => new
                    {
                        Country = c.Name,
                        States = c.States.Select(s => new
                        {
                            Name = s.Name,
                            Cities = s.Cities.Select(city => new
                            {
                                Name = city.Name
                            }).ToList()
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (country == null)
                    return NotFound("Country not found.");

                return Ok(country);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpPost("get-country-by-id")]
        public async Task<IActionResult> GetCountryById(CountryByIdModel countryById)
        {
            try
            {
                var country = await _countryRepository.GetByIdAsync(Guid.Parse(countryById.Id));
                return country == null ? NotFound("Country not found.") : Ok(new { Country = country });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPut("edit-country")]
        public async Task<IActionResult> EditCountry([FromBody] UpdateCountryModel updateDto)
        {
            if (string.IsNullOrEmpty(updateDto.Name))
            {
                return BadRequest(new { Message = "Country Name is required." });
            }
            var country = await _countryRepository.GetByIdAsync(Guid.Parse(updateDto.Id));
            if (country == null) return NotFound("Country not found.");

            // Check if User.Identity is null
            if (User?.Identity?.Name == null)
                return Unauthorized(new { Message = "User identity is not available." });

            // Check if user exists
            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(new { Message = "User name is not available." });


            var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (superuser == null) return Unauthorized("User not found.");

            country.Name = updateDto.Name;
            country.LastUpdateDate = DateTime.UtcNow;
            country.Published = true;
            country.LastUpdateUser = Guid.Parse(superuser.Id);

            _countryRepository.Update(country);
            await _countryRepository.SaveChangesAsync();

            return Ok(new { Message = "Country updated successfully", Country = country });
        }

        [Authorize]
        [HttpDelete("delete-country")]
        public async Task<IActionResult> DeleteCountry(DeleteCountryModel deleteCountry)
        {
            var country = await _countryRepository.GetByIdAsync(Guid.Parse(deleteCountry.Id));
            if (country == null) return NotFound("Country not found.");

            // Check if User.Identity is null
            if (User?.Identity?.Name == null)
                return Unauthorized(new { Message = "User identity is not available." });

            // Check if user exists
            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(new { Message = "User name is not available." });


            var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (superuser == null) return Unauthorized("User not found.");

            country.Deleted = true;
            country.Published = false;
            country.LastUpdateDate = DateTime.UtcNow;
            country.LastUpdateUser = Guid.Parse(superuser.Id);

            _countryRepository.Update(country);
            await _countryRepository.SaveChangesAsync();

            return Ok(new { Message = "Country marked as Deleted", Country = country });
        }

        #endregion
    }
}
