using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using NanoDMSAdminService.Blocks;
using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.Bank;
using NanoDMSAdminService.DTO.Campagin;
using NanoDMSAdminService.DTO.CampaignCardBin;
using NanoDMSAdminService.DTO.CardBin;
using NanoDMSAdminService.DTO.CardBrand;
using NanoDMSAdminService.Filters;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Services.Interfaces;
using NanoDMSAdminService.UnitOfWorks;
using NanoDMSSharedLibrary.CacheKeys;
using System.Text.Json;

namespace NanoDMSAdminService.Services.Implementations
{
    public class CardBinService : ICardBinService
    {
        private readonly IUnitOfWork _uow;
        private readonly IDistributedCache _cache;

        public CardBinService(IUnitOfWork uow, IDistributedCache cache)
        {
            _uow = uow;
            _cache = cache;
        }

        public async Task<IEnumerable<CardBinDto>> GetAllAsync()
        {
            const string cacheKey = "cardbins:all";

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<IEnumerable<CardBinDto>>(cached)!;

            var cardBin = await _uow.CardBins.GetAllByConditionAsync(b => !b.Deleted && b.Is_Active);

            var result =  cardBin.Select(x => new CardBinDto
            {
                Id = x.Id,
                Bank_Id = x.Bank_Id,
                Bank_Name = x.Bank?.Name,
                Card_Bin_Value = x.Card_Bin_Value,
                Card_Brand_Id = x.Card_Brand_Id,
                Card_Brand_Name = x.Card_Brand?.Name,
                Card_Type_Id = x.Card_Type_Id,
                Card_Type_Name = x.Card_Type?.Name,
                Card_Level_Id = x.Card_Level_Id,
                Card_Level_Name = x.Card_Level?.Name,
                Local_International = x.Local_International,
                Country_Id = x.Country_Id,
                Country_Name = x.Country?.Name,
                Is_Virtual = x.Is_Virtual,
                BusinessLocation_Id = x.BusinessLocation_Id,
                Business_Id = x.Business_Id,
                Is_Active = x.Is_Active,
                Deleted = x.Deleted,
                Published = x.Published,
                Create_Date = x.Create_Date,
                Create_User = x.Create_User,
                Last_Update_Date = x.Last_Update_Date,
                Last_Update_User = x.Last_Update_User,
                RecordStatus = x.RecordStatus
            });

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) });

            return result;
        }

        public async Task<CardBinDto?> GetByIdAsync(Guid id)
        {
            var cacheKey = $"cardbins:{id}";

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<CardBinDto>(cached);

            var cardBin = await _uow.CardBins.GetByIdAsync(id);
            var dto = cardBin == null ? null : MapToDto(cardBin);

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dto), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15) });

            return dto;
        }
        public async Task<List<CardBinLookupDto>> GetCardBinLookupAsync(CardBinLookupFilterModel filter)
        {
            var cacheKey =
                $"cardbin:lookup:" +
                $"{string.Join(",", filter.Bank_Ids ?? new())}|" +
                $"{string.Join(",", filter.Card_Brand_Ids ?? new())}|" +
                $"{string.Join(",", filter.Local_International_List ?? new())}";

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<List<CardBinLookupDto>>(cached)!;

            var query = _uow.CardBins.GetQueryable()
                .Where(x => !x.Deleted && x.Is_Active);

            // 🔹 Bank filter
            if (filter.Bank_Ids != null && filter.Bank_Ids.Any())
                query = query.Where(x => filter.Bank_Ids.Contains(x.Bank_Id));

            // 🔹 Card Brand / Scheme filter
            if (filter.Card_Brand_Ids != null && filter.Card_Brand_Ids.Any())
                query = query.Where(x =>
                    filter.Card_Brand_Ids.Contains(x.Card_Brand_Id));

            // 🔹 Local / International enum filter
            if (filter.Local_International_List != null &&
                filter.Local_International_List.Any())
                query = query.Where(x =>
                    filter.Local_International_List.Contains(
                        x.Local_International!.Value));

            var data = await query
                .OrderBy(x => x.Card_Bin_Value)
                .Select(x => new CardBinLookupDto
                {
                    Id = x.Id,
                    Card_Bin_Value = x.Card_Bin_Value,
                    Bank_Id = x.Bank_Id,
                    Card_Brand_Id = x.Card_Brand_Id,
                    Local_International = x.Local_International,

                    // 🔥 CONCAT VALUE
                    Products =
                        x.Card_Bin_Value + " - " +
                        x.Card_Brand!.Name + " - " +
                        x.Bank!.Name
                })
                .ToListAsync();

            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(data),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });

            return data;
        }
        public async Task<List<CardBinGroupDto>> GetGroupedCardBinsAsync(CardBinGroupedFilterModel filter)
        {
            var query = _uow.CardBins.GetQueryable()
                .Where(x => !x.Deleted && x.Is_Active);

            if (filter.Bank_Ids?.Any() == true)
                query = query.Where(x => filter.Bank_Ids.Contains(x.Bank_Id));

            if (filter.Card_Brand_Ids?.Any() == true)
                query = query.Where(x =>
                    filter.Card_Brand_Ids.Contains(x.Card_Brand_Id));

            if (filter.Local_International_List?.Any() == true)
                query = query.Where(x =>
                    filter.Local_International_List.Contains(
                        x.Local_International!.Value));

            // 🔍 Search-as-you-type (BIN / Bank / Brand)
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(x =>
                    x.Card_Bin_Value.Contains(filter.Search) ||
                    x.Bank!.Name.Contains(filter.Search) ||
                    x.Card_Brand!.Name.Contains(filter.Search));
            }

            var data = await query
                .Select(x => new
                {
                    x.Id,
                    x.Card_Bin_Value,
                    x.Local_International,
                    BankId = x.Bank_Id,
                    BankName = x.Bank!.Name,
                    BrandId = x.Card_Brand_Id,
                    BrandName = x.Card_Brand!.Name
                })
                .ToListAsync();

            var grouped = data
                .GroupBy(x => new { x.BankId, x.BankName })
                .Select(bank => new CardBinGroupDto
                {
                    Bank_Id = bank.Key.BankId,
                    Bank_Name = bank.Key.BankName,
                    Brands = bank
                        .GroupBy(b => new { b.BrandId, b.BrandName })
                        .Select(brand => new CardBrandGroupDto
                        {
                            Card_Brand_Id = brand.Key.BrandId,
                            Card_Brand_Name = brand.Key.BrandName,
                            CardBins = brand.Select(bin => new CardBinItemDto
                            {
                                Id = bin.Id,
                                Card_Bin_Value = bin.Card_Bin_Value,
                                Local_International = bin.Local_International,
                                Products =
                                    $"{bin.Card_Bin_Value} - {brand.Key.BrandName} - {bank.Key.BankName}"
                            }).ToList()
                        }).ToList()
                }).ToList();

            return grouped;
        }

        public async Task<PaginatedResponseDto<CardBinDto>> GetPagedAsync(CardBinFilterModel filter)
        {
            var cacheKey = CardBinCacheKeys.Paged(filter.PageNumber,
                filter.PageSize,
                filter.Card_Bin_Value?.Trim().ToLower() ?? "",
    filter.Bank_Id?.ToString() ?? "",
    filter.Card_Brand_Id?.ToString() ?? "",
    filter.Card_Type_Id?.ToString() ?? "",
    filter.Card_Level_Id?.ToString() ?? "",
    filter.Country_Id?.ToString() ?? "",
    filter.Local_International?.ToString() ?? "",
    filter.Is_Active?.ToString() ?? ""
                );

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<PaginatedResponseDto<CardBinDto>>(cached)!;

            var query = _uow.CardBins.GetQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Card_Bin_Value))
                query = query.Where(x => x.Card_Bin_Value.Contains(filter.Card_Bin_Value));

            if (filter.Bank_Id.HasValue)
                query = query.Where(x => x.Bank_Id == filter.Bank_Id);

            if (filter.Card_Brand_Id.HasValue)
                query = query.Where(x => x.Card_Brand_Id == filter.Card_Brand_Id);

            if (filter.Card_Type_Id.HasValue)
                query = query.Where(x => x.Card_Type_Id == filter.Card_Type_Id);

            if (filter.Card_Level_Id.HasValue)
                query = query.Where(x => x.Card_Level_Id == filter.Card_Level_Id);

            if (filter.Country_Id.HasValue)
                query = query.Where(x => x.Country_Id == filter.Country_Id);

            if (filter.Local_International.HasValue)
                query = query.Where(x => x.Local_International == filter.Local_International);

            if (filter.Is_Active.HasValue)
                query = query.Where(x => x.Is_Active == filter.Is_Active);

            query = query.OrderByDescending(x => x.Create_Date);

            var totalRecords = await query.CountAsync();

            var cardBins = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var result = new PaginatedResponseDto<CardBinDto>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / filter.PageSize),
                Data = cardBins.Select(MapToDto).ToList()
            };

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15) });

            return result;
        }
        public async Task<CardBinDto> CreateAsync(CardBinCreateDto dto, string userId)
        {
            var entity = new CardBin
            {
                Id = Guid.NewGuid(),
                Bank_Id = dto.Bank_Id,
                Card_Bin_Value = dto.Card_Bin_Value,
                Card_Brand_Id = dto.Card_Brand_Id,
                Card_Type_Id = dto.Card_Type_Id,
                Card_Level_Id = dto.Card_Level_Id,
                Local_International = dto.Local_International,
                Country_Id = dto.Country_Id,
                Is_Virtual = dto.Is_Virtual,
                RecordStatus = RecordStatus.Active,

                Published = true,
                Deleted = false,
                Is_Active = true,
                Create_Date = DateTime.UtcNow,
                Create_User = Guid.Parse(userId),
                Business_Id = Guid.Empty,
                BusinessLocation_Id = Guid.Empty
            };

            await _uow.CardBins.AddAsync(entity);
            await _uow.SaveAsync();

            // 🔥 CACHE INVALIDATION
            await _cache.RemoveAsync(CardBinCacheKeys.All);

            return await GetByIdAsync(entity.Id) ?? new CardBinDto();
        }

        public async Task<CardBinDto> UpdateAsync(Guid id, CardBinUpdateDto dto, string userId)
        {
            var entity = await _uow.CardBins.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("CardBin not found");

            entity.Bank_Id = dto.Bank_Id;
            entity.Card_Bin_Value = dto.Card_Bin_Value;
            entity.Card_Brand_Id = dto.Card_Brand_Id;
            entity.Card_Type_Id = dto.Card_Type_Id;
            entity.Card_Level_Id = dto.Card_Level_Id;
            entity.Local_International = dto.Local_International;
            entity.Country_Id = dto.Country_Id;
            entity.Is_Virtual = dto.Is_Virtual;

            entity.Last_Update_Date = DateTime.UtcNow;
            entity.Last_Update_User = Guid.Parse(userId);

            _uow.CardBins.Update(entity);
            await _uow.SaveAsync();

            // 🔥 CACHE INVALIDATION
            await _cache.RemoveAsync(CardBinCacheKeys.All);
            await _cache.RemoveAsync(CardBinCacheKeys.ById(id));

            return await GetByIdAsync(entity.Id) ?? new CardBinDto();
        }

        public async Task<CardBinDto> DeleteAsync(Guid id, string userId)
        {
            var cardBin = await _uow.CardBins.GetByIdAsync(id);
            if (cardBin == null) return new CardBinDto();

            cardBin.Deleted = true;
            cardBin.Published = false;
            cardBin.Is_Active = false;
            cardBin.RecordStatus = RecordStatus.Inactive;
            cardBin.Last_Update_Date = DateTime.UtcNow;
            cardBin.Last_Update_User = Guid.Parse(userId);

            _uow.CardBins.Update(cardBin);
            await _uow.SaveAsync();

            // 🔥 CACHE INVALIDATION
            await _cache.RemoveAsync(CardBinCacheKeys.All);
            await _cache.RemoveAsync(CardBinCacheKeys.ById(id));

            return await GetByIdAsync(cardBin.Id) ?? new CardBinDto();
        }

        private static CardBinDto MapToDto(CardBin x) => new()
        {
            Id = x.Id,
            Bank_Id = x.Bank_Id,
            Bank_Name = x.Bank?.Name,
            Card_Bin_Value = x.Card_Bin_Value,
            Card_Brand_Id = x.Card_Brand_Id,
            Card_Brand_Name = x.Card_Brand?.Name,
            Card_Type_Id = x.Card_Type_Id,
            Card_Type_Name = x.Card_Type?.Name,
            Card_Level_Id = x.Card_Level_Id,
            Card_Level_Name = x.Card_Level?.Name,
            Local_International = x.Local_International,
            Country_Id = x.Country_Id,
            Country_Name = x.Country?.Name,
            Is_Active = x.Is_Active,
            Deleted = x.Deleted,
            Published = x.Published,
            Create_Date = x.Create_Date,
            Create_User = x.Create_User,
            Last_Update_Date = x.Last_Update_Date,
            Last_Update_User = x.Last_Update_User,
            BusinessLocation_Id = x.BusinessLocation_Id,
            Business_Id = x.Business_Id,
            Start_Date = x.Start_Date,
            End_Date = x.End_Date,
            RecordStatus = x.RecordStatus
        };
    }
}
