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
    public class StateController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IStateRepository _stateRepository;

        #region Constructor
        public StateController(AppDbContext context,
            IStateRepository stateRepository,
            IConfiguration configuration,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
            _stateRepository = stateRepository;

        }
        #endregion

        #region Apis

        [Authorize]
        [HttpPost("register-state")]
        public async Task<IActionResult> RegisterState([FromBody] RegisterStateModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Name))
                    return BadRequest(new { Message = "State Name is required." });

                if (string.IsNullOrEmpty(model.CountryId))
                    return BadRequest(new { Message = "Country Id  is required." });

                // Check if User.Identity is null
                if (User?.Identity?.Name == null)
                    return Unauthorized(new { Message = "User identity is not available." });

                // Check if user exists
                var userName = User.Identity.Name;
                if (string.IsNullOrEmpty(userName))
                    return Unauthorized(new { Message = "User name is not available." });


                var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
                if (superuser == null) return Unauthorized("User not found.");

                var state = new State
                {
                    Name = model.Name,
                    CountryId = Guid.Parse(model.CountryId),
                    CreateDate = DateTime.UtcNow,
                    Published = true,
                    CreateUser = Guid.Parse(superuser.Id)
                };

                await _stateRepository.AddAsync(state);
                await _stateRepository.SaveChangesAsync();

                return Ok(new { Message = "State registered successfully", State = state });

            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }

        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-states")]
        public async Task<IActionResult> GetStates()
        {
            try
            {
                // Fetch the user profiles from the database
                var states = await _stateRepository.GetAllAsync();

                if (!states.Any()) return NotFound("No States found.");

                return Ok(new { State = states });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpPost("get-state-by-id")]
        public async Task<IActionResult> GetStateById(StateByIdModel stateById)
        {
            try
            {
                var state = await _stateRepository.GetByIdAsync(Guid.Parse(stateById.Id));

                return state == null ? NotFound("State not found.") : Ok(new { State = state });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPut("edit-state")]
        public async Task<IActionResult> EditState([FromBody] UpdateStateModel updateDto)
        {
            if (string.IsNullOrEmpty(updateDto.Name))
            {
                return BadRequest(new { Message = "State Name is required." });
            }
            var state = await _stateRepository.GetByIdAsync(Guid.Parse(updateDto.Id));
            if (state == null) return NotFound("State not found.");

            // Check if User.Identity is null
            if (User?.Identity?.Name == null)
                return Unauthorized(new { Message = "User identity is not available." });

            // Check if user exists
            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(new { Message = "User name is not available." });


            var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (superuser == null) return Unauthorized("User not found.");

            state.Name = updateDto.Name;
            state.LastUpdateDate = DateTime.UtcNow;
            state.Published = true;
            state.LastUpdateUser = Guid.Parse(superuser.Id);

            _stateRepository.Update(state);
            await _stateRepository.SaveChangesAsync();

            return Ok(new { Message = "State updated successfully", State = state });
        }

        [Authorize]
        [HttpDelete("delete-state")]
        public async Task<IActionResult> DeleteState(DeleteStateModel deleteState)
        {
            var state = await _stateRepository.GetByIdAsync(Guid.Parse(deleteState.Id));
            if (state == null) return NotFound("State not found.");

            // Check if User.Identity is null
            if (User?.Identity?.Name == null)
                return Unauthorized(new { Message = "User identity is not available." });

            // Check if user exists
            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(new { Message = "User name is not available." });


            var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (superuser == null) return Unauthorized("User not found.");

            state.Deleted = true;
            state.Published = false;
            state.LastUpdateDate = DateTime.UtcNow;
            state.LastUpdateUser = Guid.Parse(superuser.Id);

            _stateRepository.Update(state);
            await _stateRepository.SaveChangesAsync();

            return Ok(new { Message = "State marked as Deleted", State = state });
        }

        #endregion
    }
}
