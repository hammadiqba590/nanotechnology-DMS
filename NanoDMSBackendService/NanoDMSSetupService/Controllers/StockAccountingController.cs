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
    public class StockAccountingController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IStockAccountingRepository _stockAccountingRepository;

        #region Constructor
        public StockAccountingController(AppDbContext context,
           IStockAccountingRepository stockAccountingRepository,
           IConfiguration configuration,
           UserManager<IdentityUser> userManager,
           RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _stockAccountingRepository = stockAccountingRepository;
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;

        }
        #endregion

        #region Apis

        [Authorize]
        [HttpPost("register-stock-accounting")]
        public async Task<IActionResult> RegisterStockAccounting([FromBody] RegisterStockAccountingModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Name))
                    return BadRequest(new { Message = "Stock Accounting Name is required." });

                // Check if User.Identity is null
                if (User?.Identity?.Name == null)
                    return Unauthorized(new { Message = "User identity is not available." });

                // Check if user exists
                var userName = User.Identity.Name;
                if (string.IsNullOrEmpty(userName))
                    return Unauthorized(new { Message = "User name is not available." });


                var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
                if (superuser == null) return Unauthorized("User not found.");

                var stockaccounting = new StockAccountingMethod
                {
                    Name = model.Name,
                    Create_Date = DateTime.UtcNow,
                    Published = true,
                    Create_User = Guid.Parse(superuser.Id)
                };

                await _stockAccountingRepository.AddAsync(stockaccounting);
                await _stockAccountingRepository.SaveChangesAsync();

                return Ok(new { Message = "Stock Accounting registered successfully", StockAccounting = stockaccounting });

            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }

        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-stock-accountings")]
        public async Task<IActionResult> GetStockAccountings()
        {
            try
            {
                var stockaccountings = await _stockAccountingRepository.GetAllAsync();

                if (!stockaccountings.Any()) return NotFound("No Stock Accountings found.");

                return Ok(new { StockAccounting = stockaccountings });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpPost("get-stock-accounting-by-id")]
        public async Task<IActionResult> GetStockAccountingById(StockAccountingByIdModel stockAccountingById)
        {
            try
            {
                var stockaccounting = await _stockAccountingRepository.GetByIdAsync(Guid.Parse(stockAccountingById.Id));

                return stockAccountingById == null ? NotFound("Stock Accounting not found.") : Ok(new { StockAccounting = stockaccounting });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPut("edit-stock-accounting")]
        public async Task<IActionResult> EditStockAccounting([FromBody] UpdateStockAccountingModel updateDto)
        {
            if (string.IsNullOrEmpty(updateDto.Name))
            {
                return BadRequest(new { Message = "Stock Accounting Name is required." });
            }

            var stockaccounting = await _stockAccountingRepository.GetByIdAsync(Guid.Parse(updateDto.Id));
            if (stockaccounting == null) return NotFound("Stock Accounting not found.");

            // Check if User.Identity is null
            if (User?.Identity?.Name == null)
                return Unauthorized(new { Message = "User identity is not available." });

            // Check if user exists
            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(new { Message = "User name is not available." });


            var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (superuser == null) return Unauthorized("User not found.");

            stockaccounting.Name = updateDto.Name;
            stockaccounting.Last_Update_Date = DateTime.UtcNow;
            stockaccounting.Published = true;
            stockaccounting.Last_Update_User = Guid.Parse(superuser.Id);

            _stockAccountingRepository.Update(stockaccounting);
            await _stockAccountingRepository.SaveChangesAsync();

            return Ok(new { Message = "Stock Accounting updated successfully", StockAccounting = stockaccounting });
        }

        [Authorize]
        [HttpDelete("delete-stock-accounting")]
        public async Task<IActionResult> DeleteStockAccounting(DeleteStockAccountingModel deleteStockAccountingModel)
        {
            var stockaccounting = await _stockAccountingRepository.GetByIdAsync(Guid.Parse(deleteStockAccountingModel.Id));
            if (stockaccounting == null) return NotFound("Stock Accounting not found.");

            // Check if User.Identity is null
            if (User?.Identity?.Name == null)
                return Unauthorized(new { Message = "User identity is not available." });

            // Check if user exists
            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(new { Message = "User name is not available." });


            var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (superuser == null) return Unauthorized("User not found.");

            stockaccounting.Deleted = true;
            stockaccounting.Published = false;
            stockaccounting.Last_Update_Date = DateTime.UtcNow;
            stockaccounting.Last_Update_User = Guid.Parse(superuser.Id);

            _stockAccountingRepository.Update(stockaccounting);
            await _stockAccountingRepository.SaveChangesAsync();

            return Ok(new { Message = "Stock Accounting marked as Deleted", StockAccounting = stockaccounting });
        }

        #endregion
    }
}
