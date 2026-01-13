using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NanoDMSAdminService.DTO.Psp;
using NanoDMSAdminService.DTO.PspCategory;
using NanoDMSAdminService.DTO.PspCurrency;
using NanoDMSAdminService.DTO.PspDocument;
using NanoDMSAdminService.DTO.PspPaymentMethod;
using NanoDMSAdminService.Filters;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Services.Interfaces;

namespace NanoDMSAdminService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PspController : ControllerBase
    {
        private readonly IPspService _psp;
        private readonly IPspCategoryService _pspCategory;
        private readonly IPspCurrencyService _pspCurrency;
        private readonly IPspDocumentService _pspDocument;
        private readonly IPspPaymentMethodService _pspPaymentMethod;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public PspController(IPspService psp,
            IPspCategoryService pspCategory,
            IPspCurrencyService pspCurrency,
            IPspDocumentService pspDocument,
            IPspPaymentMethodService pspPaymentMethod,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _psp = psp;
            _pspCategory = pspCategory;
            _pspCurrency = pspCurrency;
            _pspDocument = pspDocument;
            _pspPaymentMethod = pspPaymentMethod;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        #region Psp

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-psps")]
        public async Task<IActionResult> GetAllPsps()
        {
            var psp = await _psp.GetAllAsync();
            return Ok(psp);
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-psp-list")]
        public async Task<IActionResult> GetList([FromQuery] PspFilterModel filter)
        {
            if (filter.PageNumber <= 0 || filter.PageSize <= 0)
                return BadRequest("Invalid pagination values");

            var result = await _psp.GetPagedAsync(filter);

            if (!result.Data.Any())
                return NoContent();

            return Ok(result);
        }

        [Authorize]
        [HttpGet("get-psp-by-id")]
        public async Task<IActionResult> GetByIdPsp(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(new { Message = "Psp Id Is Required." });

            var psp = await _psp.GetByIdAsync(id);
            if (psp == null) return NotFound();
            return Ok(new { Psp = psp });
        }

        [Authorize]
        [HttpPost("register-psp")]
        public async Task<IActionResult> Create([FromBody] PspCreateDto dto)
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

            var psp = await _psp.CreateAsync(dto, superuser.Id);

            return Ok(new { Message = "Psp Registered Succesfully", Psp = psp });
        }

        [Authorize]
        [HttpPut("update-psp")]
        public async Task<IActionResult> Update(Guid id, [FromBody] PspUpdateDto dto)
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

            var psp = await _psp.UpdateAsync(id, dto, superuser.Id);

            return Ok(new { Message = "Psp Updated Succesfully", Psp = psp });
        }

        [Authorize]
        [HttpDelete("delete-psp")]
        public async Task<IActionResult> DeletePsp(Guid id)
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

            var psp = await _psp.DeleteAsync(id, superuser.Id);

            return Ok(new { Message = "Psp Marked As Deleted", Psp = psp });
        }
        #endregion

        #region Psp Category

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-psp-categories")]
        public async Task<IActionResult> GetAllPspCategories()
        {
            var pspCategory = await _pspCategory.GetAllAsync();
            return Ok(pspCategory);
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-psp-category-list")]
        public async Task<IActionResult> GetList([FromQuery] PspCategoryFilterModel filter)
        {
            if (filter.PageNumber <= 0 || filter.PageSize <= 0)
                return BadRequest("Invalid pagination values");

            var result = await _pspCategory.GetPagedAsync(filter);

            if (!result.Data.Any())
                return NoContent();

            return Ok(result);
        }

        [Authorize]
        [HttpGet("get-psp-category-by-id")]
        public async Task<IActionResult> GetByIdPspCategory(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(new { Message = "Psp Category Id Is Required." });

            var pspCategory = await _pspCategory.GetByIdAsync(id);
            if (pspCategory == null) return NotFound();
            return Ok(new { PspCategory = pspCategory });
        }

        [Authorize]
        [HttpPost("register-psp-category")]
        public async Task<IActionResult> Create([FromBody] PspCategoryCreateDto dto)
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

            var pspCategory = await _pspCategory.CreateAsync(dto, superuser.Id);

            return Ok(new { Message = "Psp Category Registered Succesfully", PspCategory = pspCategory });
        }

        [Authorize]
        [HttpPut("update-psp-category")]
        public async Task<IActionResult> Update(Guid id, [FromBody] PspCategoryUpdateDto dto)
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

            var pspCategory = await _pspCategory.UpdateAsync(id, dto, superuser.Id);

            return Ok(new { Message = "Psp Category Updated Succesfully", PspCategory = pspCategory });
        }

        [Authorize]
        [HttpDelete("delete-psp-category")]
        public async Task<IActionResult> DeletePspCategory(Guid id)
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

            var pspCategory = await _pspCategory.DeleteAsync(id, superuser.Id);

            return Ok(new { Message = "Psp Category Marked As Deleted", PspCategory = pspCategory });
        }
        #endregion

        #region Psp Currency

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-psp-currencies")]
        public async Task<IActionResult> GetAllPspCurrencies()
        {
            var pspCurrency = await _pspCurrency.GetAllAsync();
            return Ok(pspCurrency);
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-psp-currency-list")]
        public async Task<IActionResult> GetList([FromQuery] PspCurrencyFilterModel filter)
        {
            if (filter.PageNumber <= 0 || filter.PageSize <= 0)
                return BadRequest("Invalid pagination values");

            var result = await _pspCurrency.GetPagedAsync(filter);

            if (!result.Data.Any())
                return NoContent();

            return Ok(result);
        }

        [Authorize]
        [HttpGet("get-psp-currency-by-id")]
        public async Task<IActionResult> GetByIdPspCurrency(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(new { Message = "Psp Currency Id Is Required." });

            var pspCurrency = await _pspCurrency.GetByIdAsync(id);
            if (pspCurrency == null) return NotFound();
            return Ok(new { PspCurrency = pspCurrency });
        }

        [Authorize]
        [HttpPost("register-psp-currency")]
        public async Task<IActionResult> Create([FromBody] PspCurrencyCreateDto dto)
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

            var pspCurrency = await _pspCurrency.CreateAsync(dto, superuser.Id);

            return Ok(new { Message = "Psp Currency Registered Succesfully", PspCurrency = pspCurrency });
        }

        [Authorize]
        [HttpPut("update-psp-currency")]
        public async Task<IActionResult> Update(Guid id, [FromBody] PspCurrencyUpdateDto dto)
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

            var pspCurrency = await _pspCurrency.UpdateAsync(id, dto, superuser.Id);

            return Ok(new { Message = "Psp Currency Updated Succesfully", PspCurrency = pspCurrency });
        }

        [Authorize]
        [HttpDelete("delete-psp-currency")]
        public async Task<IActionResult> DeletePspCurrency(Guid id)
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

            var pspCurrency = await _pspCurrency.DeleteAsync(id, superuser.Id);

            return Ok(new { Message = "Psp Currency Marked As Deleted", PspCurrency = pspCurrency });
        }
        #endregion

        #region Psp Document

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-psp-documents")]
        public async Task<IActionResult> GetAllPspDocuments()
        {
            var pspDocument = await _pspDocument.GetAllAsync();
            return Ok(pspDocument);
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-psp-document-list")]
        public async Task<IActionResult> GetList([FromQuery] PspDocumentFilterModel filter)
        {
            if (filter.PageNumber <= 0 || filter.PageSize <= 0)
                return BadRequest("Invalid pagination values");

            var result = await _pspDocument.GetPagedAsync(filter);

            if (!result.Data.Any())
                return NoContent();

            return Ok(result);
        }

        [Authorize]
        [HttpGet("get-psp-document-by-id")]
        public async Task<IActionResult> GetByIdPspDocument(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(new { Message = "Psp Document Id Is Required." });

            var pspDocument = await _pspDocument.GetByIdAsync(id);
            if (pspDocument == null) return NotFound();
            return Ok(new { PspDocument = pspDocument });
        }

        [Authorize]
        [HttpPost("register-psp-document")]
        public async Task<IActionResult> Create([FromBody] PspDocumentCreateDto dto)
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

            var pspDocument = await _pspDocument.CreateAsync(dto, superuser.Id);

            return Ok(new { Message = "Psp Document Registered Succesfully", PspDocument = pspDocument });
        }

        [Authorize]
        [HttpPut("update-psp-document")]
        public async Task<IActionResult> Update(Guid id, [FromBody] PspDocumentUpdateDto dto)
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

            var pspDocument = await _pspDocument.UpdateAsync(id, dto, superuser.Id);

            return Ok(new { Message = "Psp Document Updated Succesfully", PspDocument = pspDocument });
        }

        [Authorize]
        [HttpDelete("delete-psp-document")]
        public async Task<IActionResult> DeletePspDocument(Guid id)
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

            var pspDocument = await _pspDocument.DeleteAsync(id, superuser.Id);

            return Ok(new { Message = "Psp Document Marked As Deleted", PspDocument = pspDocument });
        }
        #endregion

        #region Psp Payment Method

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-psp-payment-methods")]
        public async Task<IActionResult> GetAllPspPaymentMethods()
        {
            var pspPaymentMethod = await _pspPaymentMethod.GetAllAsync();
            return Ok(pspPaymentMethod);
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-psp-payment-method-list")]
        public async Task<IActionResult> GetList([FromQuery] PspPaymentMethodFilterModel filter)
        {
            if (filter.PageNumber <= 0 || filter.PageSize <= 0)
                return BadRequest("Invalid pagination values");

            var result = await _pspPaymentMethod.GetPagedAsync(filter);

            if (!result.Data.Any())
                return NoContent();

            return Ok(result);
        }

        [Authorize]
        [HttpGet("get-psp-payment-method-by-id")]
        public async Task<IActionResult> GetByIdPspPaymentMethod(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(new { Message = "Psp Payment Method Id Is Required." });

            var pspPaymentMethod = await _pspPaymentMethod.GetByIdAsync(id);
            if (pspPaymentMethod == null) return NotFound();
            return Ok(new { PspPaymentMethod = pspPaymentMethod });
        }

        [Authorize]
        [HttpPost("register-psp-payment-method")]
        public async Task<IActionResult> Create([FromBody] PspPaymentMethodCreateDto dto)
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

            var pspPaymentMethod = await _pspPaymentMethod.CreateAsync(dto, superuser.Id);

            return Ok(new { Message = "Psp Payment Method Registered Succesfully", PspPaymentMethod = pspPaymentMethod });
        }

        [Authorize]
        [HttpPut("update-psp-payment-method")]
        public async Task<IActionResult> Update(Guid id, [FromBody] PspPaymentMethodUpdateDto dto)
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

            var pspPaymentMethod = await _pspPaymentMethod.UpdateAsync(id, dto, superuser.Id);

            return Ok(new { Message = "Psp Payment Method Updated Succesfully", PspPaymentMethod = pspPaymentMethod });
        }

        [Authorize]
        [HttpDelete("delete-psp-payment-method")]
        public async Task<IActionResult> DeletePspPaymentMethod(Guid id)
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

            var pspPaymentMethod = await _pspPaymentMethod.DeleteAsync(id, superuser.Id);

            return Ok(new { Message = "Psp Payment Method Marked As Deleted", PspPaymentMethod = pspPaymentMethod });
        }
        #endregion


    }
}
