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
    public class CurrencyController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        //private readonly ICurrencyRepository _currencyRepository;

        #region Constructor
        public CurrencyController(AppDbContext context,
           // ICurrencyRepository currencyRepository,
            IConfiguration configuration,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            //_currencyRepository = currencyRepository;
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;

        }

        #endregion

        #region Apis

        //[Authorize]
        //[HttpPost("register-currency")]
        //public async Task<IActionResult> RegisterCurrency([FromBody] RegisterCurrencyModel model)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(model.Name))
        //            return BadRequest(new { Message = "Time Zone Name is required." });

        //        if (model.ConversionRateToUSD == 0)
        //            return BadRequest(new { Message = "Conversion Rate To USD is required." });

        //        // Check if User.Identity is null
        //        if (User?.Identity?.Name == null)
        //            return Unauthorized(new { Message = "User identity is not available." });

        //        // Check if user exists
        //        var userName = User.Identity.Name;
        //        if (string.IsNullOrEmpty(userName))
        //            return Unauthorized(new { Message = "User name is not available." });


        //        var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
        //        if (superuser == null) return Unauthorized("User not found.");

        //        var currency = new Currency
        //        {
        //            Name = model.Name,
        //            ConversionRateToUSD = model.ConversionRateToUSD,
        //            CreateDate = DateTime.UtcNow,
        //            Published = true,
        //            CreateUser = Guid.Parse(superuser.Id)
        //        };

        //        await _currencyRepository.AddAsync(currency);
        //        await _currencyRepository.SaveChangesAsync();

        //        return Ok(new { Message = "Currency registered successfully", Currency = currency });

        //    }
        //    catch (Exception ex)
        //    {

        //        return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
        //    }

        //}

        //[Authorize]
        //[ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        //[HttpGet("get-currencies")]
        //public async Task<IActionResult> GetCurrencies()
        //{
        //    try
        //    {
        //        var currencies = await _currencyRepository.GetAllAsync();

        //        if (!currencies.Any()) return NotFound("No Currencies found.");

        //        return Ok(new { Currency = currencies });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }
        //}

        //[Authorize]
        //[ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        //[HttpPost("get-currency-by-id")]
        //public async Task<IActionResult> GetCurrencyById(CurrencyByIdModel currencyByIdModel)
        //{
        //    try
        //    {
        //        var currency = await _currencyRepository.GetByIdAsync(Guid.Parse(currencyByIdModel.Id));

        //        return currency == null ? NotFound("Currency not found.") : Ok(new { Currency = currency });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }
        //}

        //[Authorize]
        //[HttpPut("edit-currency")]
        //public async Task<IActionResult> EditCurrency([FromBody] UpdateCurrencyModel updateDto)
        //{
        //    if (string.IsNullOrEmpty(updateDto.Name))
        //    {
        //        return BadRequest(new { Message = "Time Zone Name is required." });
        //    }

        //    if (updateDto.ConversionRateToUSD == 0)
        //    {
        //        return BadRequest(new { Message = "Conversion Rate To USD is required." });
        //    }

        //    var currency = await _currencyRepository.GetByIdAsync(Guid.Parse(updateDto.Id));
        //    if (currency == null) return NotFound("Currency not found.");

        //    // Check if User.Identity is null
        //    if (User?.Identity?.Name == null)
        //        return Unauthorized(new { Message = "User identity is not available." });

        //    // Check if user exists
        //    var userName = User.Identity.Name;
        //    if (string.IsNullOrEmpty(userName))
        //        return Unauthorized(new { Message = "User name is not available." });


        //    var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
        //    if (superuser == null) return Unauthorized("User not found.");

        //    currency.Name = updateDto.Name;
        //    currency.ConversionRateToUSD = updateDto.ConversionRateToUSD;
        //    currency.LastUpdateDate = DateTime.UtcNow;
        //    currency.Published = true;
        //    currency.LastUpdateUser = Guid.Parse(superuser.Id);

        //    _currencyRepository.Update(currency);
        //    await _currencyRepository.SaveChangesAsync();

        //    return Ok(new { Message = "Currency updated successfully", Currency = currency });
        //}

        //[Authorize]
        //[HttpDelete("delete-currency")]
        //public async Task<IActionResult> DeleteCurrency(DeleteCurrencyModel deleteCurrencyModel)
        //{
        //    var currency = await _currencyRepository.GetByIdAsync(Guid.Parse(deleteCurrencyModel.Id));
        //    if (currency == null) return NotFound("Currency not found.");

        //    // Check if User.Identity is null
        //    if (User?.Identity?.Name == null)
        //        return Unauthorized(new { Message = "User identity is not available." });

        //    // Check if user exists
        //    var userName = User.Identity.Name;
        //    if (string.IsNullOrEmpty(userName))
        //        return Unauthorized(new { Message = "User name is not available." });


        //    var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
        //    if (superuser == null) return Unauthorized("User not found.");

        //    currency.Deleted = true;
        //    currency.Published = false;
        //    currency.LastUpdateDate = DateTime.UtcNow;
        //    currency.LastUpdateUser = Guid.Parse(superuser.Id);

        //    _currencyRepository.Update(currency);
        //    await _currencyRepository.SaveChangesAsync();

        //    return Ok(new { Message = "Currency marked as Deleted", Currency = currency });
        //}

        #endregion

    }
}
