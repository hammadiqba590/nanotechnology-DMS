using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NanoDMSAdminService.DTO.CardBin;
using NanoDMSAdminService.DTO.CardBrand;
using NanoDMSAdminService.DTO.CardLevel;
using NanoDMSAdminService.DTO.CardType;
using NanoDMSAdminService.Filters;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Services.Interfaces;

namespace NanoDMSAdminService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardController : ControllerBase
    {
        private readonly ICardBinService _cardBin;
        private readonly ICardBrandService _cardBrand;
        private readonly ICardLevelService _cardLevel;
        private readonly ICardTypeService _cardType;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CardController(
            ICardBinService cardBin,
            ICardBrandService cardBrand,
            ICardLevelService cardLevel,
            ICardTypeService cardType,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _cardBin = cardBin;
            _cardBrand = cardBrand;
            _cardLevel = cardLevel;
            _cardType = cardType;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        #region CardBin

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-card-bins")]
        public async Task<IActionResult> GetAllCardBins()
        {
            var cardBin = await _cardBin.GetAllAsync();
            return Ok(cardBin);
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-card-bin-list")]
        public async Task<IActionResult> GetList([FromQuery] CardBinFilterModel filter)
        {
            if (filter.PageNumber <= 0 || filter.PageSize <= 0)
                return BadRequest("Invalid pagination values");

            var result = await _cardBin.GetPagedAsync(filter);

            if (!result.Data.Any())
                return NoContent();

            return Ok(result);
        }

        [Authorize]
        [HttpGet("get-card-bin-by-id")]
        public async Task<IActionResult> GetByIdCardBin(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(new { Message = "Card Bin Id Is Required." });

            var cardBin = await _cardBin.GetByIdAsync(id);
            if (cardBin == null) return NotFound();
            return Ok(new { CardBin = cardBin });
        }

        [Authorize]
        [HttpPost("register-card-bin")]
        public async Task<IActionResult> Create([FromBody] CardBinCreateDto dto)
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

            var cardBin = await _cardBin.CreateAsync(dto, superuser.Id);

            return Ok(new { Message = "Card Bin Registered Succesfully", CardBin = cardBin });
        }

        [Authorize]
        [HttpPut("update-card-bin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CardBinUpdateDto dto)
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

            var cardBin = await _cardBin.UpdateAsync(id, dto, superuser.Id);

            return Ok(new { Message = "Card Bin Updated Succesfully", CardBin = cardBin });
        }

        [Authorize]
        [HttpDelete("delete-card-bin")]
        public async Task<IActionResult> DeleteCardBin(Guid id)
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

            var cardBin = await _cardBin.DeleteAsync(id, superuser.Id);

            return Ok(new { Message = "Card Bin Marked As Deleted", CardBin = cardBin });
        }
        #endregion

        #region Card Brand

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-card-brands")]
        public async Task<IActionResult> GetAllCardBrands()
        {
            var cardBrand = await _cardBrand.GetAllAsync();
            return Ok(cardBrand);
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-card-brand-list")]
        public async Task<IActionResult> GetList([FromQuery] CardBrandFilterModel filter)
        {
            if (filter.PageNumber <= 0 || filter.PageSize <= 0)
                return BadRequest("Invalid pagination values");

            var result = await _cardBrand.GetPagedAsync(filter);

            if (!result.Data.Any())
                return NoContent();

            return Ok(result);
        }

        [Authorize]
        [HttpGet("get-card-brand-by-id")]
        public async Task<IActionResult> GetByIdCardBrand(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(new { Message = "Card Brand Id Is Required." });

            var cardBrand = await _cardBrand.GetByIdAsync(id);
            if (cardBrand == null) return NotFound();
            return Ok(new { CardBin = cardBrand });
        }

        [Authorize]
        [HttpPost("register-card-brand")]
        public async Task<IActionResult> Create([FromBody] CardBrandCreateDto dto)
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

            var cardBrand = await _cardBrand.CreateAsync(dto, superuser.Id);

            return Ok(new { Message = "Card Brand Registered Succesfully", CardBrand = cardBrand });
        }

        [Authorize]
        [HttpPut("update-card-brand")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CardBrandUpdateDto dto)
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

            var cardBrand = await _cardBrand.UpdateAsync(id, dto, superuser.Id);

            return Ok(new { Message = "Card Brand Updated Succesfully", CardBrand = cardBrand });
        }

        [Authorize]
        [HttpDelete("delete-card-brand")]
        public async Task<IActionResult> DeleteCardBrand(Guid id)
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

            var cardBrand = await _cardBrand.DeleteAsync(id, superuser.Id);

            return Ok(new { Message = "Card Brand Marked As Deleted", CardBrand = cardBrand });
        }
        #endregion

        #region Card Level

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-card-levels")]
        public async Task<IActionResult> GetAllCardLevels()
        {
            var cardLevel = await _cardLevel.GetAllAsync();
            return Ok(cardLevel);
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-card-level-list")]
        public async Task<IActionResult> GetList([FromQuery] CardLevelFilterModel filter)
        {
            if (filter.PageNumber <= 0 || filter.PageSize <= 0)
                return BadRequest("Invalid pagination values");

            var result = await _cardLevel.GetPagedAsync(filter);

            if (!result.Data.Any())
                return NoContent();

            return Ok(result);
        }

        [Authorize]
        [HttpGet("get-card-level-by-id")]
        public async Task<IActionResult> GetByIdCardLevel(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(new { Message = "Card Level Id Is Required." });

            var cardLevel = await _cardLevel.GetByIdAsync(id);
            if (cardLevel == null) return NotFound();
            return Ok(new { CardLevel = cardLevel });
        }

        [Authorize]
        [HttpPost("register-card-level")]
        public async Task<IActionResult> Create([FromBody] CardLevelCreateDto dto)
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

            var cardLevel = await _cardLevel.CreateAsync(dto, superuser.Id);

            return Ok(new { Message = "Card Level Registered Succesfully", CardLevel = cardLevel });
        }

        [Authorize]
        [HttpPut("update-card-level")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CardLevelUpdateDto dto)
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

            var cardLevel = await _cardLevel.UpdateAsync(id, dto, superuser.Id);

            return Ok(new { Message = "Card Level Updated Succesfully", CardLevel = cardLevel });
        }

        [Authorize]
        [HttpDelete("delete-card-level")]
        public async Task<IActionResult> DeleteCardLevel(Guid id)
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

            var cardLevel = await _cardLevel.DeleteAsync(id, superuser.Id);

            return Ok(new { Message = "Card Level Marked As Deleted", CardLevel = cardLevel });
        }
        #endregion

        #region Card Type

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-card-types")]
        public async Task<IActionResult> GetAllCardTypes()
        {
            var cardType = await _cardType.GetAllAsync();
            return Ok(cardType);
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-card-type-list")]
        public async Task<IActionResult> GetList([FromQuery] CardTypeFilterModel filter)
        {
            if (filter.PageNumber <= 0 || filter.PageSize <= 0)
                return BadRequest("Invalid pagination values");

            var result = await _cardType.GetPagedAsync(filter);

            if (!result.Data.Any())
                return NoContent();

            return Ok(result);
        }

        [Authorize]
        [HttpGet("get-card-type-by-id")]
        public async Task<IActionResult> GetByIdCardType(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(new { Message = "Card Type Id Is Required." });

            var cardType = await _cardType.GetByIdAsync(id);
            if (cardType == null) return NotFound();
            return Ok(new { CardType = cardType });
        }

        [Authorize]
        [HttpPost("register-card-type")]
        public async Task<IActionResult> Create([FromBody] CardTypeCreateDto dto)
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

            var cardType = await _cardType.CreateAsync(dto, superuser.Id);

            return Ok(new { Message = "Card Type Registered Succesfully", CardType = cardType });
        }

        [Authorize]
        [HttpPut("update-card-type")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CardTypeUpdateDto dto)
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

            var cardType = await _cardType.UpdateAsync(id, dto, superuser.Id);

            return Ok(new { Message = "Card Type Updated Succesfully", CardType = cardType });
        }

        [Authorize]
        [HttpDelete("delete-card-type")]
        public async Task<IActionResult> DeleteCardType(Guid id)
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

            var cardType = await _cardType.DeleteAsync(id, superuser.Id);

            return Ok(new { Message = "Card Type Marked As Deleted", CardType = cardType });
        }
        #endregion


    }
}
