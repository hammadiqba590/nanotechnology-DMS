using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using NanoDMSAdminService.Blocks;
using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.CampaignBank;
using NanoDMSAdminService.DTO.CampaignCardBin;
using NanoDMSAdminService.Filters;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Services.Interfaces;
using NanoDMSAdminService.UnitOfWorks;
using NanoDMSSharedLibrary.CacheKeys;
using System.Text.Json;

namespace NanoDMSAdminService.Services.Implementations
{
    public class CampaignCardBinService : ICampaignCardBinService
    {
        private readonly IUnitOfWork _uow;
        private readonly IDistributedCache _cache;
        public CampaignCardBinService(IUnitOfWork uow, IDistributedCache cache)
        {
            _uow = uow;
            _cache = cache;
        }

        public async Task<IEnumerable<CampaignCardBinDto>> GetAllAsync()
        {
            const string cacheKey = "campaigncardbins:all";

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<IEnumerable<CampaignCardBinDto>>(cached)!;

            var campaignCardBin = await _uow.CampaignCardBins.GetAllByConditionAsync(b => !b.Deleted && b.Is_Active);

            var result = campaignCardBin.Select(x => new CampaignCardBinDto
            {
                Id = x.Id,
                Campagin_Bank_Id = x.Campagin_Bank_Id,
                Card_Bin_Id = x.Card_Bin_Id,
                Status = x.Status,
                Is_Active = x.Is_Active,
                Deleted = x.Deleted,
                Published = x.Published,
                Business_Id = x.Business_Id,
                BusinessLocation_Id = x.BusinessLocation_Id,
                Create_Date = x.Create_Date,
                Create_User = x.Create_User,
                Last_Update_Date = x.Last_Update_Date,
                Last_Update_User = x.Last_Update_User,
                RecordStatus = x.RecordStatus
            });

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) });

            return result;
        }

        public async Task<CampaignCardBinDto?> GetByIdAsync(Guid id)
        {
            var cacheKey = $"campaigncardbins:{id}";

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<CampaignCardBinDto>(cached);

            var campaignCardBin = await _uow.CampaignCardBins.GetByIdAsync(id);
            var dto = campaignCardBin == null ? null : MapToDto(campaignCardBin);

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dto), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15) });

            return dto;
        }

        public async Task<PaginatedResponseDto<CampaignCardBinDto>> GetPagedAsync(CampaignCardBinFilterModel filter)
        {

            var cacheKey = CampaignCardBinCacheKeys.Paged(filter.PageNumber, filter.PageSize, filter.Status?.ToString() ?? "");

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<PaginatedResponseDto<CampaignCardBinDto>>(cached)!;

            var query = _uow.CampaignCardBins.GetQueryable(); 

            if (filter.Status.HasValue)
                query = query.Where(x => x.Status == filter.Status);

            query = query.OrderByDescending(x => x.Create_Date);

            var totalRecords = await query.CountAsync();

            var campaigns = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var result = new PaginatedResponseDto<CampaignCardBinDto>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / filter.PageSize),
                Data = campaigns.Select(MapToDto).ToList()
            };

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15) });

            return result;
        }

        public async Task<CampaignCardBinDto> CreateAsync(CampaignCardBinCreateDto dto, string userId)
        {
            var entity = new CampaignCardBin
            {
                Id = Guid.NewGuid(),
                Campagin_Bank_Id = dto.Campagin_Bank_Id,
                Card_Bin_Id = dto.Card_Bin_Id,
                Status = dto.Status ?? CampaginCardBinStatus.Active,
                RecordStatus = RecordStatus.Active,

                Published = true,
                Deleted = false,
                Is_Active = true,
                Create_Date = DateTime.UtcNow,
                Create_User = Guid.Parse(userId),
                Business_Id = dto.Business_Id,
                BusinessLocation_Id = dto.Business_Location_Id
            };

            await _uow.CampaignCardBins.AddAsync(entity);
            await _uow.SaveAsync();

            // 🔥 CACHE INVALIDATION
            await _cache.RemoveAsync(CampaignCardBinCacheKeys.All);


            return MapToDto(entity);
        }

        public async Task<CampaignCardBinDto> UpdateAsync(Guid id, CampaignCardBinUpdateDto dto, string userId)
        {
            var entity = await _uow.CampaignCardBins.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Campaign CardBin not found");

            entity.Campagin_Bank_Id = dto.Campagin_Bank_Id;
            entity.Card_Bin_Id = dto.Card_Bin_Id;
            entity.Status = dto.Status;

            entity.Last_Update_Date = DateTime.UtcNow;
            entity.Last_Update_User = Guid.Parse(userId);

            _uow.CampaignCardBins.Update(entity);
            await _uow.SaveAsync();

            // 🔥 CACHE INVALIDATION
            await _cache.RemoveAsync(CampaignCardBinCacheKeys.All);
            await _cache.RemoveAsync(CampaignCardBinCacheKeys.ById(id));

            return MapToDto(entity);
        }

        public async Task<CampaignCardBinDto> DeleteAsync(Guid id, string userId)
        {
            var campaignCardBin = await _uow.CampaignCardBins.GetByIdAsync(id);
            if (campaignCardBin == null) return new CampaignCardBinDto();

            campaignCardBin.Deleted = true;
            campaignCardBin.Published = false;
            campaignCardBin.Is_Active = false;
            campaignCardBin.Status = CampaginCardBinStatus.Inactive;
            campaignCardBin.RecordStatus = RecordStatus.Inactive;
            campaignCardBin.Last_Update_Date = DateTime.UtcNow;
            campaignCardBin.Last_Update_User = Guid.Parse(userId);

            _uow.CampaignCardBins.Update(campaignCardBin);
            await _uow.SaveAsync();

            // 🔥 CACHE INVALIDATION
            await _cache.RemoveAsync(CampaignCardBinCacheKeys.All);
            await _cache.RemoveAsync(CampaignCardBinCacheKeys.ById(id));


            return MapToDto(campaignCardBin);
        }
        private static CampaignCardBinDto MapToDto(CampaignCardBin x) => new()
        {
            Id = x.Id,
            Campagin_Bank_Id = x.Campagin_Bank_Id,
            Card_Bin_Id = x.Card_Bin_Id,
            Status = x.Status,
            Business_Id = x.Business_Id,
            BusinessLocation_Id = x.BusinessLocation_Id,
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
