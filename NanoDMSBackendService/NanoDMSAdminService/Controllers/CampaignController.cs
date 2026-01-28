using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NanoDMSAdminService.DTO.Campagin;
using NanoDMSAdminService.DTO.CampaignBank;
using NanoDMSAdminService.DTO.CampaignCardBin;
using NanoDMSAdminService.Filters;
using NanoDMSAdminService.Services.Implementations;
using NanoDMSAdminService.Services.Interfaces;

namespace NanoDMSAdminService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignController : ControllerBase
    {
        private readonly ICampaignService _campaign;
        private readonly ICampaignBankService _campaignBank;
        private readonly ICampaignCardBinService _campaignCardBin;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CampaignController(ICampaignService campaign,
            ICampaignBankService campaignBank,
            ICampaignCardBinService campaignCardBin,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _campaign = campaign;
            _campaignBank = campaignBank;
            _campaignCardBin = campaignCardBin;
        }

        #region Campaign

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-campaigns")]
        public async Task<IActionResult> GetAllCampaigns()
        {
            var campaigns = await _campaign.GetAllAsync();
            return Ok(campaigns);
        }

        [Authorize]
        [HttpGet("active-campaign-by-serial")]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> GetActiveCampaigns(string serialNumber)
        {
            if (string.IsNullOrWhiteSpace(serialNumber))
                return BadRequest("Terminal serial number is required");

            var campaigns = await _campaign.GetActiveCampaignsByTerminalAsync(serialNumber);

            if (!campaigns.Any())
                return NoContent();

            return Ok(campaigns);
        }

        [Authorize]
        [HttpPost("create-full-campaign")]
        public async Task<IActionResult> CreateFullCampaign([FromBody] CampaignFullCreateDto dto)
        {
            var user = await _userManager.FindByNameAsync(User.Identity!.Name!);
            if (user == null) return Unauthorized();

            var campaign = await _campaign.CreateFullCampaignAsync(dto, Guid.Parse(user.Id));

            return Ok(new
            {
                Message = "Campaign created successfully",
                Data = campaign
            });
        }


        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-campaign-list")]
        public async Task<IActionResult> GetList([FromQuery] CampaignFilterModel filter)
        {
            if (filter.PageNumber <= 0 || filter.PageSize <= 0)
                return BadRequest("Invalid pagination values");

            var result = await _campaign.GetPagedAsync(filter);

            if (!result.Data.Any())
                return NoContent();

            return Ok(result);
        }

        [Authorize]
        [HttpGet("get-campaign-by-id")]
        public async Task<IActionResult> GetByIdCampaign(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(new { Message = "Campaign Id Is Required." });

            var campaign = await _campaign.GetByIdAsync(id);
            if (campaign == null) return NotFound();
            return Ok(new { Campaign = campaign });
        }

        [Authorize]
        [HttpPost("register-campaign")]
        public async Task<IActionResult> Create([FromBody] CampaignCreateDto dto)
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

            var campaign = await _campaign.CreateAsync(dto, superuser.Id);

            return Ok(new { Message = "Campaign Registered Succesfully", Campaign = campaign });
        }

        [Authorize]
        [HttpPut("update-campaign")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CampaignUpdateDto dto)
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

            var campaign = await _campaign.UpdateAsync(id, dto, superuser.Id);

            return Ok(new { Message = "Campaign Updated Succesfully", Country = campaign });
        }

        [Authorize]
        [HttpDelete("delete-campaign")]
        public async Task<IActionResult> DeleteCampaign(Guid id)
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

            var campaign = await _campaign.DeleteAsync(id, superuser.Id);

            return Ok(new { Message = "Campaign Marked As Deleted", Campaign = campaign });
        }
        #endregion

        #region Campaign Bank

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-campaign-banks")]
        public async Task<IActionResult> GetAllCampaignBanks()
        {
            var campaignBanks = await _campaignBank.GetAllAsync();
            return Ok(campaignBanks);
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-campaign-bank-list")]
        public async Task<IActionResult> GetList([FromQuery] CampaignBankFilterModel filter)
        {
            if (filter.PageNumber <= 0 || filter.PageSize <= 0)
                return BadRequest("Invalid pagination values");

            var result = await _campaignBank.GetPagedAsync(filter);

            if (!result.Data.Any())
                return NoContent();

            return Ok(result);
        }

        [Authorize]
        [HttpGet("get-campaign-bank-by-id")]
        public async Task<IActionResult> GetByIdCampaignBank(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(new { Message = "Campaign Bank Id Is Required." });

            var campaignBank = await _campaignBank.GetByIdAsync(id);
            if (campaignBank == null) return NotFound();
            return Ok(new { CampaignBank = campaignBank });
        }

        [Authorize]
        [HttpPost("register-campaign-bank")]
        public async Task<IActionResult> Create([FromBody] CampaignBankCreateDto dto)
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

            var campaignBank = await _campaignBank.CreateAsync(dto, superuser.Id);

            return Ok(new { Message = "Campaign Bank Registered Succesfully", CampaignBank = campaignBank });
        }

        [Authorize]
        [HttpPut("update-campaign-bank")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CampaignBankUpdateDto dto)
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

            var campaignBank = await _campaignBank.UpdateAsync(id, dto, superuser.Id);

            return Ok(new { Message = "Campaign Bank Updated Succesfully", CampaignBank = campaignBank });
        }

        [Authorize]
        [HttpDelete("delete-campaign-bank")]
        public async Task<IActionResult> DeleteCampaignBank(Guid id)
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

            var campaignBank = await _campaignBank.DeleteAsync(id, superuser.Id);

            return Ok(new { Message = "Campaign Bank Marked As Deleted", CampaignBank = campaignBank });
        }
        #endregion

        #region Campaign Card Bin

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-campaign-card-bins")]
        public async Task<IActionResult> GetAllCampaignCardBins()
        {
            var campaignCardBin = await _campaignCardBin.GetAllAsync();
            return Ok(campaignCardBin);
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-campaign-card-bin-list")]
        public async Task<IActionResult> GetList([FromQuery] CampaignCardBinFilterModel filter)
        {
            if (filter.PageNumber <= 0 || filter.PageSize <= 0)
                return BadRequest("Invalid pagination values");

            var result = await _campaignCardBin.GetPagedAsync(filter);

            if (!result.Data.Any())
                return NoContent();

            return Ok(result);
        }

        [Authorize]
        [HttpGet("get-campaign-card-bin-by-id")]
        public async Task<IActionResult> GetByIdCampaignCardBin(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(new { Message = "Campaign Card Bin Id Is Required." });

            var campaignCardBin = await _campaignCardBin.GetByIdAsync(id);
            if (campaignCardBin == null) return NotFound();
            return Ok(new { CampaignCardBin = campaignCardBin });
        }

        [Authorize]
        [HttpPost("register-campaign-card-bin")]
        public async Task<IActionResult> Create([FromBody] CampaignCardBinCreateDto dto)
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

            var campaignCardBin = await _campaignCardBin.CreateAsync(dto, superuser.Id);

            return Ok(new { Message = "Campaign Card Bin Registered Succesfully", CampaignCardBin = campaignCardBin });
        }

        [Authorize]
        [HttpPut("update-campaign-card-bin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CampaignCardBinUpdateDto dto)
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

            var campaignCardBin = await _campaignCardBin.UpdateAsync(id, dto, superuser.Id);

            return Ok(new { Message = "Campaign Card Bin Updated Succesfully", CampaignCardBin = campaignCardBin });
        }

        [Authorize]
        [HttpDelete("delete-campaign-card-bin")]
        public async Task<IActionResult> DeleteCampaignCardBin(Guid id)
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

            var campaignCardBin = await _campaignCardBin.DeleteAsync(id, superuser.Id);

            return Ok(new { Message = "Campaign Card Bin Marked As Deleted", CampaignCardBin = campaignCardBin });
        }
        #endregion
    }
}
