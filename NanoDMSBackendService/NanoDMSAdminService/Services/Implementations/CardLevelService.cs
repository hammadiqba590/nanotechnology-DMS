using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using NanoDMSAdminService.Blocks;
using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.CardBrand;
using NanoDMSAdminService.DTO.CardLevel;
using NanoDMSAdminService.Filters;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Services.Interfaces;
using NanoDMSAdminService.UnitOfWorks;
using NanoDMSSharedLibrary.CacheKeys;
using System.Text.Json;

namespace NanoDMSAdminService.Services.Implementations
{
    public class CardLevelService : ICardLevelService
    {
        private readonly IUnitOfWork _uow;
        private readonly IDistributedCache _cache;
        public CardLevelService(IUnitOfWork uow, IDistributedCache cache)
        {
            _uow = uow;
            _cache = cache;
        }

        public async Task<IEnumerable<CardLevelDto>> GetAllAsync()
        {
            const string cacheKey = "cardlevels:all";

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<IEnumerable<CardLevelDto>>(cached)!;

            var cardLevel = await _uow.CardLevels.GetAllByConditionAsync(b => !b.Deleted && b.Is_Active);

            var result = cardLevel.Select(x => new CardLevelDto
            {
                Id = x.Id,
                Name = x.Name,
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

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)});

            return result;
        }

        public async Task<CardLevelDto?> GetByIdAsync(Guid id)
        {
            var cacheKey = $"cardlevels:{id}";

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<CardLevelDto>(cached);

            var cardLevel = await _uow.CardLevels.GetByIdAsync(id);
            var dto = cardLevel == null ? null : MapToDto(cardLevel);

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dto), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)});

            return dto;
        }

        public async Task<PaginatedResponseDto<CardLevelDto>> GetPagedAsync(CardLevelFilterModel filter)
        {
            var cacheKey = CardLevelCacheKeys.Paged(filter.PageNumber, filter.PageSize);

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<PaginatedResponseDto<CardLevelDto>>(cached)!;

            var query = _uow.CardLevels.GetQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Name))
                query = query.Where(x => x.Name.Contains(filter.Name));

            query = query.OrderByDescending(x => x.Create_Date);

            var totalRecords = await query.CountAsync();

            var cardLevels = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var result = new PaginatedResponseDto<CardLevelDto>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / filter.PageSize),
                Data = cardLevels.Select(MapToDto).ToList()
            };

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15) });

            return result;
        }

        public async Task<CardLevelDto> CreateAsync(CardLevelCreateDto dto, string userId)
        {
            var entity = new CardLevel
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                RecordStatus = RecordStatus.Active,
                Published = true,
                Deleted = false,
                Is_Active = true,
                Create_Date = DateTime.UtcNow,
                Create_User = Guid.Parse(userId),
                Business_Id = dto.Business_Id,
                BusinessLocation_Id = dto.Business_Location_Id
            };

            await _uow.CardLevels.AddAsync(entity);
            await _uow.SaveAsync();

            // 🔥 CACHE INVALIDATION
            await _cache.RemoveAsync(CardLevelCacheKeys.All);

            return MapToDto(entity);
        }

        public async Task<CardLevelDto> UpdateAsync(Guid id, CardLevelUpdateDto dto, string userId)
        {
            var entity = await _uow.CardLevels.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Card Level not found");

            entity.Name = dto.Name;

            entity.Last_Update_Date = DateTime.UtcNow;
            entity.Last_Update_User = Guid.Parse(userId);

            _uow.CardLevels.Update(entity);
            await _uow.SaveAsync();

            // 🔥 CACHE INVALIDATION
            await _cache.RemoveAsync(CardLevelCacheKeys.All);
            await _cache.RemoveAsync(CardLevelCacheKeys.ById(id));

            return MapToDto(entity);
        }

        public async Task<CardLevelDto> DeleteAsync(Guid id, string userId)
        {
            var cardLevel = await _uow.CardLevels.GetByIdAsync(id);
            if (cardLevel == null) return new CardLevelDto();

            cardLevel.Deleted = true;
            cardLevel.Published = false;
            cardLevel.Is_Active = false;
            cardLevel.RecordStatus = RecordStatus.Inactive;
            cardLevel.Last_Update_Date = DateTime.UtcNow;
            cardLevel.Last_Update_User = Guid.Parse(userId);

            _uow.CardLevels.Update(cardLevel);
            await _uow.SaveAsync();

            // 🔥 CACHE INVALIDATION
            await _cache.RemoveAsync(CardLevelCacheKeys.All);
            await _cache.RemoveAsync(CardLevelCacheKeys.ById(id));

            return MapToDto(cardLevel);
        }

        private static CardLevelDto MapToDto(CardLevel x) => new()
        {
            Id = x.Id,
            Name = x.Name,
            Is_Active = x.Is_Active,
            Deleted = x.Deleted,
            Published = x.Published,
            Create_Date = x.Create_Date,
            Create_User = x.Create_User,
            Last_Update_Date = x.Last_Update_Date,
            Last_Update_User = x.Last_Update_User,
            RecordStatus = x.RecordStatus
        };
    }
}
