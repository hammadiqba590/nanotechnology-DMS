using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NanoDMSBusinessService.Data;
using NanoDMSBusinessService.DTO;
using NanoDMSBusinessService.Models;
using NanoDMSBusinessService.Repositories;

namespace NanoDMSBusinessService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessConfigController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IBusinessConfigRepository _businessconfigRepository;


        #region Constructor
        public BusinessConfigController(AppDbContext context,
            IBusinessConfigRepository businessConfigRepository,
            IConfiguration configuration,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _businessconfigRepository = businessConfigRepository;
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;

        }
        #endregion

        #region Apis

        [Authorize]
        [HttpPost("register-business-config")]
        public async Task<IActionResult> RegisterBusinessConfig([FromBody] RegisterBusinessConfigModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.NameKey))
                    return BadRequest(new { Message = "Name Key is required." });

                if (string.IsNullOrEmpty(model.ConfigValue))
                    return BadRequest(new { Message = "Config Value is required." });

                if (string.IsNullOrEmpty(model.ConfigType))
                    return BadRequest(new { Message = "Config Type is required." });

                if (model.BusinessId == Guid.Empty)
                    return BadRequest(new { Message = "Business Id is required." });

                if (model.BusinessLocationId == Guid.Empty)
                    return BadRequest(new { Message = "Business Location Id is required." });

                if (User?.Identity?.Name == null)
                    return Unauthorized("User identity is not available.");

                var userName = User.Identity.Name;
                if (string.IsNullOrEmpty(userName))
                    return Unauthorized("User name is not available.");

                // Fix for CS8604: Ensure userName is not null before passing it to FindByNameAsync
                var superuser = await _userManager.FindByNameAsync(userName ?? string.Empty);
                if (superuser == null)
                    return Unauthorized("User not found.");

                var businessConfig = new BusinessConfig
                {
                    NameKey = model.NameKey,
                    ConfigValue = model.ConfigValue,
                    ConfigType = model.ConfigType,
                    Description = model.Description,
                    BusinessId = model.BusinessId,
                    BusinessLocationId = model.BusinessLocationId,
                    CreateDate = DateTime.UtcNow,
                    Published = true,
                    CreateUser = Guid.Parse(superuser.Id)
                };

                await _businessconfigRepository.AddAsync(businessConfig);
                await _businessconfigRepository.SaveChangesAsync();

                return Ok(new { Message = "BusinessConfig registered successfully", BusinessConfig = businessConfig });

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-business-configs")]
        public async Task<IActionResult> GetBusinessConfigs()
        {
            var configKeys = new List<string>
            {
                "Product.CategoryLevel",

            };

            var configValues = await _businessconfigRepository.GetConfigValuesAsync(configKeys);
            return Ok(new { BusinessConfig = configValues });
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpPost("get-business-config-by-id")]
        public async Task<IActionResult> GetBusinessConfigById(BusinessConfigByIdModel businessConfigById)
        {
            try
            {
                var businessconfig = await _businessconfigRepository.GetByIdAsync(businessConfigById.Id);
                return businessconfig == null ? NotFound("BusinessConfig not found.") :
                Ok(new { BusinessConfig = businessconfig });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPut("edit-business-config")]
        public async Task<IActionResult> EditBusinessConfig([FromBody] UpdateBusinessConfigModel updateDto)
        {
            if (string.IsNullOrEmpty(updateDto.NameKey))
            {
                return BadRequest(new { Message = "Name Key is required." });
            }

            if (string.IsNullOrEmpty(updateDto.ConfigValue))
            {
                return BadRequest(new { Message = "Config Value is required." });
            }

            if (string.IsNullOrEmpty(updateDto.ConfigType))
            {
                return BadRequest(new { Message = "Config Type is required." });
            }
            if (updateDto.BusinessId == Guid.Empty)
            {
                return BadRequest(new { Message = "Business Id is required." });
            }
            if (updateDto.BusinessLocationId == Guid.Empty)
            {
                return BadRequest(new { Message = "Business Location Id is required." });
            }

            var businessconfig = await _businessconfigRepository.GetByIdAsync(updateDto.Id);
            if (businessconfig == null) return NotFound("BusinessConfig not found.");

            if (User?.Identity?.Name == null)
                return Unauthorized("User identity is not available.");

            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized("User name is not available.");

            var superuser = await _userManager.FindByNameAsync(userName);
            if (superuser == null) return Unauthorized("User not found.");

            businessconfig.NameKey = updateDto.NameKey;
            businessconfig.ConfigValue = updateDto.ConfigValue;
            businessconfig.ConfigType = updateDto.ConfigType;
            businessconfig.Description = updateDto.Description;
            businessconfig.BusinessId = updateDto.BusinessId;
            businessconfig.BusinessLocationId = updateDto.BusinessLocationId;

            businessconfig.LastUpdateDate = DateTime.UtcNow;
            businessconfig.Published = true;
            businessconfig.LastUpdateUser = Guid.Parse(superuser.Id);

            _businessconfigRepository.Update(businessconfig);
            await _businessconfigRepository.SaveChangesAsync();

            return Ok(new { Message = "Business Config updated successfully", BusinessConfig = businessconfig });
        }

        [Authorize]
        [HttpDelete("delete-business-config")]
        public async Task<IActionResult> DeleteBusinessConfig(DeleteBusinessConfigModel deleteBusinessConfigModel)
        {
            var businessconfig = await _businessconfigRepository.GetByIdAsync(deleteBusinessConfigModel.Id);
            if (businessconfig == null) return NotFound("BusinessConfig not found.");

            // Fix for CS8602: Ensure User.Identity.Name is not null before accessing it
            var userName = User?.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized("User name is not available.");

            var superuser = await _userManager.FindByNameAsync(userName ?? string.Empty);
            if (superuser == null) return Unauthorized("User not found.");

            businessconfig.Deleted = true;
            businessconfig.Published = false;
            businessconfig.LastUpdateDate = DateTime.UtcNow;
            businessconfig.LastUpdateUser = Guid.Parse(superuser.Id);

            _businessconfigRepository.Update(businessconfig);
            await _businessconfigRepository.SaveChangesAsync();

            return Ok(new { Message = "BusinessConfig marked as Deleted", BusinessConfig = businessconfig });
        }

        #endregion

    }
}
