using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NanoDMSAdminService.DTO.CardBin;
using NanoDMSAdminService.DTO.PosTerminalAssignment;
using NanoDMSAdminService.DTO.PosTerminalConfiguration;
using NanoDMSAdminService.DTO.PosTerminalMaster;
using NanoDMSAdminService.DTO.PosTerminalStatusHistory;
using NanoDMSAdminService.Filters;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Services.Interfaces;

namespace NanoDMSAdminService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PosTerminalController : ControllerBase
    {
        private readonly IPosTerminalMasterService _posTerminal;
        private readonly IPosTerminalAssignmentService _posTerminalAssignment;
        private readonly IPosTerminalConfigurationService _posTerminalConfiguration;
        private readonly IPosTerminalStatusHistoryService _posTerminalStatusHistory;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public PosTerminalController(IPosTerminalMasterService posTerminal,
            IPosTerminalAssignmentService posTerminalAssignment,
            IPosTerminalConfigurationService posTerminalConfiguration,
            IPosTerminalStatusHistoryService posTerminalStatusHistory,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _posTerminal = posTerminal;
            _posTerminalAssignment = posTerminalAssignment;
            _posTerminalConfiguration = posTerminalConfiguration;
            _posTerminalStatusHistory = posTerminalStatusHistory;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        #region Pos Terminal Master

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-pos-terminals")]
        public async Task<IActionResult> GetAllPosTerminals()
        {
            var terminal = await _posTerminal.GetAllAsync();
            return Ok(terminal);
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-pos-terminal-list")]
        public async Task<IActionResult> GetList([FromQuery] PosTerminalMasterFilterModel filter)
        {
            if (filter.PageNumber <= 0 || filter.PageSize <= 0)
                return BadRequest("Invalid pagination values");

            var result = await _posTerminal.GetPagedAsync(filter);

            if (!result.Data.Any())
                return NoContent();

            return Ok(result);
        }

        [Authorize]
        [HttpGet("get-pos-terminal-by-id")]
        public async Task<IActionResult> GetByIdPosTerminal(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(new { Message = "Pos Terminal Id Is Required." });

            var terminal = await _posTerminal.GetByIdAsync(id);
            if (terminal == null) return NotFound();
            return Ok(new { PosTerminal = terminal });
        }

        [Authorize]
        [HttpPost("register-pos-terminal")]
        public async Task<IActionResult> Create([FromBody] PosTerminalMasterCreateDto dto)
        {
            if (User?.Identity?.Name == null)
                return Unauthorized(new { Message = "User identity Is Not Available." });

            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(new { Message = "User name Is Not Available." });

            var superuser = await _userManager.FindByNameAsync(userName);
            if (superuser == null)
                return Unauthorized(new { Message = "User Not Found." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var terminal = await _posTerminal.CreateAsync(dto, superuser.Id);

            return Ok(new { Message = "Pos Terminal Registered Succesfully", PosTerminal = terminal });
        }

        [Authorize]
        [HttpPut("update-pos-terminal")]
        public async Task<IActionResult> Update(Guid id, [FromBody] PosTerminalMasterUpdateDto dto)
        {
            if (User?.Identity?.Name == null)
                return Unauthorized(new { Message = "User identity Is Not Available." });

            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(new { Message = "User name Is Not Available." });

            var superuser = await _userManager.FindByNameAsync(userName);
            if (superuser == null)
                return Unauthorized(new { Message = "User Not Found." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var terminal = await _posTerminal.UpdateAsync(id, dto, superuser.Id);

            return Ok(new { Message = "Pos Terminal Updated Succesfully", PosTerminal = terminal });
        }

        [Authorize]
        [HttpDelete("delete-pos-terminal")]
        public async Task<IActionResult> DeletePosTerminal(Guid id)
        {
            // Check if User.Identity is null
            if (User?.Identity?.Name == null)
                return Unauthorized(new { Message = "User Identity Is Not Available." });

            // Check if user exists
            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(new { Message = "User Name Is Not Available." });

            var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (superuser == null) return Unauthorized("User Not Found.");

            var terminal = await _posTerminal.DeleteAsync(id, superuser.Id);

            return Ok(new { Message = "Pos Terminal Marked As Deleted", PosTerminal = terminal });
        }
        #endregion

        #region Pos Terminal Assignment

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-pos-terminal-assignments")]
        public async Task<IActionResult> GetAllPosTerminalAssignments()
        {
            var terminalAssignment = await _posTerminalAssignment.GetAllAsync();
            return Ok(terminalAssignment);
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-pos-terminal-assignment-list")]
        public async Task<IActionResult> GetList([FromQuery] PosTerminalAssignmentFilterModel filter)
        {
            if (filter.PageNumber <= 0 || filter.PageSize <= 0)
                return BadRequest("Invalid pagination values");

            var result = await _posTerminalAssignment.GetPagedAsync(filter);

            if (!result.Data.Any())
                return NoContent();

            return Ok(result);
        }

        [Authorize]
        [HttpGet("get-pos-terminal-assignment-by-id")]
        public async Task<IActionResult> GetByIdPosTerminalAssignment(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(new { Message = "Pos Terminal Assignment Id Is Required." });

            var terminalAssignment = await _posTerminalAssignment.GetByIdAsync(id);
            if (terminalAssignment == null) return NotFound();
            return Ok(new { PosTerminalAssignment = terminalAssignment });
        }

        [Authorize]
        [HttpPost("register-pos-terminal-assignment")]
        public async Task<IActionResult> Create([FromBody] PosTerminalAssignmentCreateDto dto)
        {
            if (User?.Identity?.Name == null)
                return Unauthorized(new { Message = "User identity Is Not Available." });

            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(new { Message = "User name Is Not Available." });

            var superuser = await _userManager.FindByNameAsync(userName);
            if (superuser == null)
                return Unauthorized(new { Message = "User Not Found." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var terminalAssignment = await _posTerminalAssignment.CreateAsync(dto, superuser.Id);

            return Ok(new { Message = "Pos Terminal Assignment Registered Succesfully", PosTerminalAssignment = terminalAssignment });
        }

        [Authorize]
        [HttpPut("update-pos-terminal-assignment")]
        public async Task<IActionResult> Update(Guid id, [FromBody] PosTerminalAssignmentUpdateDto dto)
        {
            if (User?.Identity?.Name == null)
                return Unauthorized(new { Message = "User identity Is Not Available." });

            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(new { Message = "User name Is Not Available." });

            var superuser = await _userManager.FindByNameAsync(userName);
            if (superuser == null)
                return Unauthorized(new { Message = "User Not Found." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var terminalAssignment = await _posTerminalAssignment.UpdateAsync(id, dto, superuser.Id);

            return Ok(new { Message = "Pos Terminal Assignment Updated Succesfully", PosTerminalAssignment = terminalAssignment });
        }

        [Authorize]
        [HttpDelete("delete-pos-terminal-assignment")]
        public async Task<IActionResult> DeletePosTerminalAssignment(Guid id)
        {
            // Check if User.Identity is null
            if (User?.Identity?.Name == null)
                return Unauthorized(new { Message = "User Identity Is Not Available." });

            // Check if user exists
            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(new { Message = "User Name Is Not Available." });

            var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (superuser == null) return Unauthorized("User Not Found.");

            var terminalAssignment = await _posTerminalAssignment.DeleteAsync(id, superuser.Id);

            return Ok(new { Message = "Pos Terminal Assignment Marked As Deleted", PosTerminalAssignment = terminalAssignment });
        }
        #endregion

        #region Pos Terminal Configuration

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-pos-terminal-configurations")]
        public async Task<IActionResult> GetAllPosTerminalConfigurations()
        {
            var terminalConfiguration = await _posTerminalConfiguration.GetAllAsync();
            return Ok(terminalConfiguration);
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-pos-terminal-configuration-list")]
        public async Task<IActionResult> GetList([FromQuery] PosTerminalConfigurationFilterModel filter)
        {
            if (filter.PageNumber <= 0 || filter.PageSize <= 0)
                return BadRequest("Invalid pagination values");

            var result = await _posTerminalConfiguration.GetPagedAsync(filter);

            if (!result.Data.Any())
                return NoContent();

            return Ok(result);
        }

        [Authorize]
        [HttpGet("get-pos-terminal-configuration-by-id")]
        public async Task<IActionResult> GetByIdPosTerminalConfiguration(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(new { Message = "Pos Terminal Configuration Id Is Required." });

            var terminalConfiguration = await _posTerminalConfiguration.GetByIdAsync(id);
            if (terminalConfiguration == null) return NotFound();
            return Ok(new { PosTerminalConfiguration = terminalConfiguration });
        }

        [Authorize]
        [HttpPost("register-pos-terminal-configuration")]
        public async Task<IActionResult> Create([FromBody] PosTerminalConfigurationCreateDto dto)
        {
            if (User?.Identity?.Name == null)
                return Unauthorized(new { Message = "User identity Is Not Available." });

            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(new { Message = "User name Is Not Available." });

            var superuser = await _userManager.FindByNameAsync(userName);
            if (superuser == null)
                return Unauthorized(new { Message = "User Not Found." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var terminalConfiguration = await _posTerminalConfiguration.CreateAsync(dto, superuser.Id);

            return Ok(new { Message = "Pos Terminal Configuration Registered Succesfully", PosTerminalConfiguration = terminalConfiguration });
        }

        [Authorize]
        [HttpPut("update-pos-terminal-configuration")]
        public async Task<IActionResult> Update(Guid id, [FromBody] PosTerminalConfigurationUpdateDto dto)
        {
            if (User?.Identity?.Name == null)
                return Unauthorized(new { Message = "User identity Is Not Available." });

            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(new { Message = "User name Is Not Available." });

            var superuser = await _userManager.FindByNameAsync(userName);
            if (superuser == null)
                return Unauthorized(new { Message = "User Not Found." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var terminalConfiguration = await _posTerminalConfiguration.UpdateAsync(id, dto, superuser.Id);

            return Ok(new { Message = "Pos Terminal Configuration Updated Succesfully", PosTerminalConfiguration = terminalConfiguration });
        }

        [Authorize]
        [HttpDelete("delete-pos-terminal-configuration")]
        public async Task<IActionResult> DeletePosTerminalConfiguration(Guid id)
        {
            // Check if User.Identity is null
            if (User?.Identity?.Name == null)
                return Unauthorized(new { Message = "User Identity Is Not Available." });

            // Check if user exists
            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(new { Message = "User Name Is Not Available." });

            var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (superuser == null) return Unauthorized("User Not Found.");

            var terminalConfiguration = await _posTerminalConfiguration.DeleteAsync(id, superuser.Id);

            return Ok(new { Message = "Pos Terminal Configuration Marked As Deleted", PosTerminalConfiguration = terminalConfiguration });
        }
        #endregion

        #region Pos Terminal Status History
        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-pos-terminal-status-histories")]
        public async Task<IActionResult> GetAllPosTerminalStatusHistories()
        {
            var terminalStatusHistory = await _posTerminalStatusHistory.GetAllAsync();
            return Ok(terminalStatusHistory);
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-pos-terminal-status-history-list")]
        public async Task<IActionResult> GetList([FromQuery] PosTerminalStatusHistoryFilterModel filter)
        {
            if (filter.PageNumber <= 0 || filter.PageSize <= 0)
                return BadRequest("Invalid pagination values");

            var result = await _posTerminalStatusHistory.GetPagedAsync(filter);

            if (!result.Data.Any())
                return NoContent();

            return Ok(result);
        }

        [Authorize]
        [HttpGet("get-pos-terminal-status-history-by-id")]
        public async Task<IActionResult> GetByIdPosTerminalStatusHistory(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(new { Message = "Pos Terminal Status History Id Is Required." });

            var terminalStatusHistory = await _posTerminalStatusHistory.GetByIdAsync(id);
            if (terminalStatusHistory == null) return NotFound();
            return Ok(new { PosTerminalStatusHistory = terminalStatusHistory });
        }

        [Authorize]
        [HttpPost("register-pos-terminal-status-history")]
        public async Task<IActionResult> Create([FromBody] PosTerminalStatusHistoryCreateDto dto)
        {
            if (User?.Identity?.Name == null)
                return Unauthorized(new { Message = "User identity Is Not Available." });

            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(new { Message = "User name Is Not Available." });

            var superuser = await _userManager.FindByNameAsync(userName);
            if (superuser == null)
                return Unauthorized(new { Message = "User Not Found." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var terminalStatusHistory = await _posTerminalStatusHistory.CreateAsync(dto, superuser.Id);

            return Ok(new { Message = "Pos Terminal Status History Registered Succesfully", PosTerminalStatusHistory = terminalStatusHistory });
        }

        [Authorize]
        [HttpPut("update-pos-terminal-status-history")]
        public async Task<IActionResult> Update(Guid id, [FromBody] PosTerminalStatusHistoryUpdateDto dto)
        {
            if (User?.Identity?.Name == null)
                return Unauthorized(new { Message = "User identity Is Not Available." });

            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(new { Message = "User name Is Not Available." });

            var superuser = await _userManager.FindByNameAsync(userName);
            if (superuser == null)
                return Unauthorized(new { Message = "User Not Found." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var terminalStatusHistory = await _posTerminalStatusHistory.UpdateAsync(id, dto, superuser.Id);

            return Ok(new { Message = "Pos Terminal Status History Updated Succesfully", PosTerminalStatusHistory = terminalStatusHistory });
        }

        [Authorize]
        [HttpDelete("delete-pos-terminal-status-history")]
        public async Task<IActionResult> DeletePosTerminalStatusHistory(Guid id)
        {
            // Check if User.Identity is null
            if (User?.Identity?.Name == null)
                return Unauthorized(new { Message = "User Identity Is Not Available." });

            // Check if user exists
            var userName = User.Identity.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(new { Message = "User Name Is Not Available." });

            var superuser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (superuser == null) return Unauthorized("User Not Found.");

            var terminalStatusHistory = await _posTerminalStatusHistory.DeleteAsync(id, superuser.Id);

            return Ok(new { Message = "Pos Terminal Status History Marked As Deleted", PosTerminalStatusHistory = terminalStatusHistory });
        }
        #endregion
    }
}
