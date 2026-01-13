using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NanoDMSAdminService.DTO.Bank;
using NanoDMSAdminService.DTO.Country;
using NanoDMSAdminService.DTO.Currency;
using NanoDMSAdminService.DTO.DiscountRule;
using NanoDMSAdminService.DTO.DiscountRuleHistory;
using NanoDMSAdminService.Filters;
using NanoDMSAdminService.Services.Implementations;
using NanoDMSAdminService.Services.Interfaces;

namespace NanoDMSAdminService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MasterEntryController : ControllerBase
    {
        private readonly IBankService _service;
        private readonly ICountryService _countryservice;
        private readonly ICurrencyService _currencyservice;
        private readonly IDiscountRuleService _ruleservice;
        private readonly IDiscountRuleHistoryService _rulehistoryservice;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public MasterEntryController(IBankService service,
            ICountryService countryService,
            ICurrencyService currencyService,
            IDiscountRuleService ruleService, 
            IDiscountRuleHistoryService ruleHistoryService,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _service = service;
            _countryservice = countryService;
            _currencyservice = currencyService;
            _ruleservice = ruleService;
            _rulehistoryservice = ruleHistoryService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        #region Bank

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-banks")]
        public async Task<IActionResult> GetAll()
        {
            var banks = await _service.GetAllAsync();
            return Ok(banks);
        }

        [Authorize]
        [HttpGet("get-bank-by-id")]
        public async Task<IActionResult> GetById(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(new { Message = "Bank Id Is Required." });

            var bank = await _service.GetByIdAsync(id);
            if (bank == null) return NotFound();
            return Ok(new { Bank = bank });
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-bank-list")]
        public async Task<IActionResult> GetBankList([FromQuery] BankFilterModel filter)
        {
            if (filter.PageNumber <= 0 || filter.PageSize <= 0)
                return BadRequest("Invalid pagination values");

            var result = await _service.GetPagedAsync(filter);

            if (!result.Data.Any())
                return NoContent();

            return Ok(result);
        }

        [Authorize]
        [HttpPost("register-bank")]
        public async Task<IActionResult> Create([FromBody] BankCreateDto dto)
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


            

            var bank = await _service.CreateAsync(dto, superuser.Id);
            return Ok(new { Message = "Bank Registered Successfully", Bank = bank });
        }

        [Authorize]
        [HttpPut("update-bank")]
        public async Task<IActionResult> Update(Guid id, [FromBody] BankUpdateDto dto)
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

            


            var updated = await _service.UpdateAsync(id, dto, superuser.Id);
            if (updated == null)
                throw new KeyNotFoundException("Bank not found");

            return Ok(new { Message = "Bank Updated Successfully!", Bank = updated });
        }

        [Authorize]
        [HttpDelete("delete-bank")]
        public async Task<IActionResult> Delete(Guid id)
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


            var deleted = await _service.DeleteAsync(id, superuser.Id);
            if (deleted == null)
                throw new KeyNotFoundException("Bank not found");

            return Ok(new { Message = "Bank Marked As Deleted", Bank = deleted });
        }

        #endregion

        #region Country

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-countries")]
        public async Task<IActionResult> GetAllCountries()
        {
            var country = await _countryservice.GetAllAsync();
            return Ok(country);
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-country-list")]
        public async Task<IActionResult> GetList([FromQuery] CountryFilterModel filter)
        {
            if (filter.PageNumber <= 0 || filter.PageSize <= 0)
                return BadRequest("Invalid pagination values");

            var result = await _countryservice.GetPagedAsync(filter);

            if (!result.Data.Any())
                return NoContent();

            return Ok(result);
        }

        [Authorize]
        [HttpGet("get-country-by-id")]
        public async Task<IActionResult> GetByIdCountry(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(new { Message = "Country Id Is Required." });

            var country = await _countryservice.GetByIdAsync(id);
            if (country == null) return NotFound();
            return Ok(new { Country = country });
        }

        [Authorize]
        [HttpPost("register-country")]
        public async Task<IActionResult> Create([FromBody] CountryCreateDto dto)
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


            

            var country = await _countryservice.CreateAsync(dto, superuser.Id);

            return Ok(new { Message = "Country Registered Succesfully",Country = country});
        }

        [Authorize]
        [HttpPut("update-country")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CountryUpdateDto dto)
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

            var country = await _countryservice.UpdateAsync(id, dto, superuser.Id);

            return Ok(new {Message = "Country Updated Succesfully",Country = country });
        }

        [Authorize]
        [HttpDelete("delete-country")]
        public async Task<IActionResult> DeleteCountry(Guid id)
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

            var country = await _countryservice.DeleteAsync(id, superuser.Id);

            return Ok(new {Message = "Country Marked As Deleted",Country = country });
        }

        #endregion

        #region Currency

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-currencies")]
        public async Task<IActionResult> GetAllCurrencies()
        {
            var currencies = await _currencyservice.GetAllAsync();
            return Ok(currencies);
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-currency-list")]
        public async Task<IActionResult> GetList([FromQuery] CurrencyFilterModel filter) 
        {
            if (filter.PageNumber <= 0 || filter.PageSize <= 0)
                return BadRequest("Invalid pagination values");

            var result = await _currencyservice.GetPagedAsync(filter);

            if (!result.Data.Any())
                return NoContent();

            return Ok(result);
        }

        [Authorize]
        [HttpGet("get-currency-by-id")]
        public async Task<IActionResult> GetByIdCurrency(Guid id) 
        {
            if (id == Guid.Empty)
                return BadRequest(new { Message = "Currency Id Is Required." });

            var currency = await _currencyservice.GetByIdAsync(id);
            if (currency == null) return NotFound();
            return Ok(new { Currency = currency });
        }

        [Authorize]
        [HttpPost("register-currency")]
        public async Task<IActionResult> Create([FromBody]CurrencyCreateDto dto)
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

            var currency = await _currencyservice.CreateAsync(dto, superuser.Id);

            return Ok(new { Message = "Currency Registered Succesfully", Currency = currency });
        }
        [Authorize]
        [HttpPut("update-currency")]
        public async Task<IActionResult> Update(Guid id, CurrencyUpdateDto dto)
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

            var currency = await _currencyservice.UpdateAsync(id, dto, superuser.Id);

            return Ok(new { Message = "Currency Updated Succesfully", Currency = currency });
        }


        [Authorize]
        [HttpDelete("delete-currency")]
        public async Task<IActionResult> DeleteCurrency(Guid id)
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

            var currency = await _currencyservice.DeleteAsync(id, superuser.Id);

            return Ok(new { Message = "Currency Marked As Deleted", Currency = currency });
        }


        #endregion

        #region Discount Rule

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-discount-rules")]
        public async Task<IActionResult> GetAllDiscountRules()
        {
            var rules = await _ruleservice.GetAllAsync();
            return Ok(rules);
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-discount-rule-list")]
        public async Task<IActionResult> GetList([FromQuery] DiscountRuleFilterModel filter)
        {
            if (filter.PageNumber <= 0 || filter.PageSize <= 0)
                return BadRequest("Invalid pagination values");

            var result = await _ruleservice.GetPagedAsync(filter);

            if (!result.Data.Any())
                return NoContent();

            return Ok(result);
        }

        [Authorize]
        [HttpGet("get-discount-rule-by-id")]
        public async Task<IActionResult> GetByIdDiscountRule(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(new { Message = "Discount Rule Id Is Required." });

            var rule = await _ruleservice.GetByIdAsync(id);
            if (rule == null) return NotFound();
            return Ok(new { DiscountRule = rule });
        }

        [Authorize]
        [HttpPost("register-discount-rule")]
        public async Task<IActionResult> Create([FromBody] DiscountRuleCreateDto dto)
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

            var rule = await _ruleservice.CreateAsync(dto, superuser.Id);

            return Ok(new { Message = "Discount Rule Registered Succesfully", DiscountRule = rule });
        }

        [Authorize]
        [HttpPut("update-discount-rule")]
        public async Task<IActionResult> Update(Guid id, DiscountRuleUpdateDto dto)
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

            var rule = await _ruleservice.UpdateAsync(id, dto, superuser.Id);

            return Ok(new { Message = "Discount Rule Updated Succesfully", DiscountRule = rule });
        }


        [Authorize]
        [HttpDelete("delete-discount-rule")]
        public async Task<IActionResult> DeleteDiscountRule(Guid id)
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

            var rule = await _ruleservice.DeleteAsync(id, superuser.Id);

            return Ok(new { Message = "Discount Rule Marked As Deleted", DiscountRule = rule });
        }

        #endregion

        #region Discount Rule History

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-discount-rule-histories")]
        public async Task<IActionResult> GetAllDiscountRuleHistories()
        {
            var rules = await _rulehistoryservice.GetAllAsync();
            return Ok(rules);
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-discount-rule-history-list")]
        public async Task<IActionResult> GetList([FromQuery] DiscountRuleHistoryFilterModel filter)
        {
            if (filter.PageNumber <= 0 || filter.PageSize <= 0)
                return BadRequest("Invalid pagination values");

            var result = await _rulehistoryservice.GetPagedAsync(filter);

            if (!result.Data.Any())
                return NoContent();

            return Ok(result);
        }

        [Authorize]
        [HttpGet("get-discount-rule-history-by-id")]
        public async Task<IActionResult> GetByIdDiscountRuleHistory(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest(new { Message = "Discount Rule History Id Is Required." });

            var rule = await _rulehistoryservice.GetByIdAsync(id);
            if (rule == null) return NotFound();
            return Ok(new { DiscountRuleHistory = rule });
        }

        [Authorize]
        [HttpPost("register-discount-rule-history")]
        public async Task<IActionResult> Create([FromBody] DiscountRuleHistoryCreateDto dto)
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

            var rule = await _rulehistoryservice.CreateAsync(dto, Guid.Parse(superuser.Id));

            return Ok(new { Message = "Discount Rule History Registered Succesfully", DiscountRuleHistory = rule });
        }

        [Authorize]
        [HttpPut("update-discount-rule-history")]
        public async Task<IActionResult> Update(Guid id, DiscountRuleHistoryUpdateDto dto)
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

            var rule = await _rulehistoryservice.UpdateAsync(id, dto, Guid.Parse(superuser.Id));

            return Ok(new { Message = "Discount Rule History Updated Succesfully", DiscountRuleHistory = rule });
        }

        [Authorize]
        [HttpDelete("delete-discount-rule-history")]
        public async Task<IActionResult> DeleteDiscountRuleHistory(Guid id)
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

            var rule = await _rulehistoryservice.DeleteAsync(id, Guid.Parse(superuser.Id));

            return Ok(new { Message = "Discount Rule History Marked As Deleted", DiscountRuleHistory = rule });
        }
        #endregion


    }
}
