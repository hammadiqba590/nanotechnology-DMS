using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NanoDMSAdminService.Blocks;
using NanoDMSAdminService.Common;
using NanoDMSAdminService.Data;
using NanoDMSAdminService.DTO.CardBin;
using NanoDMSAdminService.DTO.CardBrand;
using NanoDMSAdminService.DTO.CardLevel;
using NanoDMSAdminService.DTO.CardType;
using NanoDMSAdminService.Filters;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Services.Interfaces;
using System.Globalization;

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
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CardController(
            ICardBinService cardBin,
            ICardBrandService cardBrand,
            ICardLevelService cardLevel,
            ICardTypeService cardType,
            AppDbContext context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _cardBin = cardBin;
            _cardBrand = cardBrand;
            _cardLevel = cardLevel;
            _cardType = cardType;
            _context = context;
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
        [HttpGet("get-card-bin-lookup")]
        public async Task<IActionResult> CardBinLookup([FromQuery] CardBinLookupFilterModel filter)
        {
            var result = await _cardBin.GetCardBinLookupAsync(filter);

            if (!result.Any())
                return NoContent();

            return Ok(result);
        }

        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        [HttpGet("get-card-bin-grouped")]
        public async Task<IActionResult> GetGroupedCardBins([FromQuery] CardBinGroupedFilterModel filter)
        {
            var result = await _cardBin.GetGroupedCardBinsAsync(filter);

            if (!result.Any())
                return NoContent();

            return Ok(result);
        }

        [Authorize]
        [HttpPost("bulk-upload-card-bin")]
        public async Task<ActionResult<CardBinBulkUploadResponse>> BulkUploadCsvOptimized(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("CSV file is required");

            // --- 1️⃣ Parse CSV ---
            List<CardBinCsvRowDto> rows;
            using (var stream = file.OpenReadStream())
            using (var reader = new StreamReader(stream))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<CardBinCsvRowMap>();
                rows = csv.GetRecords<CardBinCsvRowDto>()
                          .Select(r => new CardBinCsvRowDto
                          {
                              Bin = r.Bin?.Trim() ?? "",
                              IssuingBank = r.IssuingBank?.Trim() ?? "N/A",
                              CardBrand = r.CardBrand?.Trim() ?? "N/A",
                              CardType = r.CardType?.Trim() ?? "N/A",
                              CardLevel = r.CardLevel?.Trim() ?? "N/A",
                              Country = r.Country?.Trim() ?? "N/A",
                              LocalInternational = r.LocalInternational?.Trim() ?? ""
                          })
                          .ToList();
            }

            var response = new CardBinBulkUploadResponse { TotalRows = rows.Count };

            // --- 2️⃣ Wrap everything inside EF Core execution strategy for PostgreSQL ---
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // --- 3️⃣ Collect unique reference values from CSV ---
                    var countriesCsv = rows.Select(r => string.IsNullOrWhiteSpace(r.Country) ? "N/A" : r.Country).Distinct().ToList();
                    var banksCsv = rows.Select(r => string.IsNullOrWhiteSpace(r.IssuingBank) ? "N/A" : r.IssuingBank).Distinct().ToList();
                    var brandsCsv = rows.Select(r => string.IsNullOrWhiteSpace(r.CardBrand) ? "N/A" : r.CardBrand).Distinct().ToList();
                    var typesCsv = rows.Select(r => string.IsNullOrWhiteSpace(r.CardType) ? "N/A" : r.CardType).Distinct().ToList();
                    var levelsCsv = rows.Select(r => string.IsNullOrWhiteSpace(r.CardLevel) ? "N/A" : r.CardLevel).Distinct().ToList();

                    // --- 4️⃣ Load existing reference tables ---
                    var countriesDb = await _context.Countries.Where(c => countriesCsv.Contains(c.Name)).ToDictionaryAsync(c => c.Name, c => c);
                    var banksDb = await _context.Banks.Include(b => b.Country).Where(b => banksCsv.Contains(b.Name)).ToDictionaryAsync(b => b.Name, b => b);
                    var brandsDb = await _context.CardBrands.Where(b => brandsCsv.Contains(b.Name)).ToDictionaryAsync(b => b.Name, b => b);
                    var typesDb = await _context.CardTypes.Where(t => typesCsv.Contains(t.Name)).ToDictionaryAsync(t => t.Name, t => t);
                    var levelsDb = await _context.CardLevels.Where(l => levelsCsv.Contains(l.Name)).ToDictionaryAsync(l => l.Name, l => l);

                    // --- 5️⃣ Ensure "N/A" entries exist ---
                    T EnsureNa<T>(Dictionary<string, T> cache, Func<T> createFunc) where T : BaseEntity
                    {
                        if (!cache.ContainsKey("N/A"))
                        {
                            var entry = createFunc();
                            _context.Set<T>().Add(entry);
                            cache["N/A"] = entry;
                            return entry;
                        }
                        return cache["N/A"];
                    }

                    var naCountry = EnsureNa(countriesDb, () => new Country { Id = Guid.NewGuid(), Name = "N/A", Iso2 = "NA", Iso3 = "NA" });
                    var naBank = EnsureNa(banksDb, () => new Bank
                    {
                        Id = Guid.NewGuid(),
                        Name = "N/A",
                        Short_Name = "N/A",
                        Short_Code = Guid.NewGuid().ToString("N")[..6],
                        Country_Id = naCountry.Id
                    });
                    var naBrand = EnsureNa(brandsDb, () => new CardBrand { Id = Guid.NewGuid(), Name = "N/A" });
                    var naType = EnsureNa(typesDb, () => new CardType { Id = Guid.NewGuid(), Name = "N/A" });
                    var naLevel = EnsureNa(levelsDb, () => new CardLevel { Id = Guid.NewGuid(), Name = "N/A" });

                    await _context.SaveChangesAsync(); // Save N/A entries to get IDs

                    // --- 6️⃣ Create missing reference entries from CSV ---
                    void CreateMissing<T>(List<string> keys, Dictionary<string, T> cache, Func<string, T> createFunc) where T : BaseEntity
                    {
                        foreach (var key in keys)
                        {
                            if (!cache.ContainsKey(key))
                            {
                                var entry = createFunc(key);
                                _context.Set<T>().Add(entry);
                                cache[key] = entry;
                            }
                        }
                    }

                    CreateMissing(countriesCsv, countriesDb, k => new Country { Id = Guid.NewGuid(), Name = k, Iso2 = "NA", Iso3 = "NA" });
                    CreateMissing(banksCsv, banksDb, k => new Bank
                    {
                        Id = Guid.NewGuid(),
                        Name = k,
                        Short_Name = k.Length > 20 ? k[..20] : k,
                        Short_Code = Guid.NewGuid().ToString("N")[..6],
                        Country_Id = naCountry.Id
                    });
                    CreateMissing(brandsCsv, brandsDb, k => new CardBrand { Id = Guid.NewGuid(), Name = k });
                    CreateMissing(typesCsv, typesDb, k => new CardType { Id = Guid.NewGuid(), Name = k });
                    CreateMissing(levelsCsv, levelsDb, k => new CardLevel { Id = Guid.NewGuid(), Name = k });

                    await _context.SaveChangesAsync(); // Save all missing reference entries

                    // --- 7️⃣ Load existing BINs once for duplication check ---
                    var csvBins = rows.Select(r => r.Bin).ToList();
                    var existingBins = await _context.CardBins
                                                    .Where(cb => csvBins.Contains(cb.Card_Bin_Value))
                                                    .Select(cb => cb.Card_Bin_Value)
                                                    .ToHashSetAsync();

                    // --- 8️⃣ Prepare CardBins in memory ---
                    var cardBinsToInsert = new List<CardBin>();

                    foreach (var row in rows)
                    {
                        var result = new CardBinUploadResult { Bin = row.Bin };

                        try
                        {
                            if (string.IsNullOrWhiteSpace(row.Bin))
                                throw new Exception("BIN is required");
                            if (!row.Bin.All(char.IsDigit) || row.Bin.Length < 6 || row.Bin.Length > 12)
                                throw new Exception("BIN must be 6–12 digits");
                            if (existingBins.Contains(row.Bin))
                                throw new Exception("Duplicate BIN");

                            var country = countriesDb.GetValueOrDefault(row.Country) ?? naCountry;
                            var bank = banksDb.GetValueOrDefault(row.IssuingBank) ?? naBank;
                            var brand = brandsDb.GetValueOrDefault(row.CardBrand) ?? naBrand;
                            var type = typesDb.GetValueOrDefault(row.CardType) ?? naType;
                            var levelKey = string.IsNullOrWhiteSpace(row.CardLevel) ? "N/A" : row.CardLevel;
                            var level = levelsDb.GetValueOrDefault(levelKey) ?? naLevel;

                            var localInternational = row.LocalInternational?.ToLower() switch
                            {
                                "local" => LocalInternationalStatus.Local,
                                "international" => LocalInternationalStatus.International,
                                _ => LocalInternationalStatus.Unknown
                            };

                            cardBinsToInsert.Add(new CardBin
                            {
                                Id = Guid.NewGuid(),
                                Card_Bin_Value = row.Bin,
                                Bank_Id = bank.Id,
                                Card_Brand_Id = brand.Id,
                                Card_Type_Id = type.Id,
                                Card_Level_Id = level.Id,
                                Country_Id = country.Id,
                                Local_International = localInternational,
                                Is_Virtual = false
                            });

                            result.Success = true;
                        }
                        catch (Exception ex)
                        {
                            result.Success = false;
                            result.ErrorMessage = ex.Message;
                        }

                        response.RowResults.Add(result);
                    }

                    // --- 9️⃣ Bulk insert all CardBins ---
                    if (cardBinsToInsert.Any())
                        await _context.CardBins.AddRangeAsync(cardBinsToInsert);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    response.ImportedRows = response.RowResults.Count(r => r.Success);
                    response.FailedRows = response.RowResults.Count(r => !r.Success);

                    return Ok(response);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    // Get full details including inner exceptions
                    var fullMessage = ex.ToString(); // includes inner exceptions stack
                    return StatusCode(500, new { Message = "Import failed", Details = fullMessage });
                }
            });
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
