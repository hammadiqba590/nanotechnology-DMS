using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.PosTerminalAssignment;
using NanoDMSAdminService.DTO.PosTerminalConfiguration;
using NanoDMSAdminService.Filters;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Services.Interfaces;
using NanoDMSAdminService.UnitOfWorks;
using NanoDMSSharedLibrary.CacheKeys;
using System.Text.Json;

namespace NanoDMSAdminService.Services.Implementations
{
    public class PosTerminalConfigurationService : IPosTerminalConfigurationService
    {
        private readonly IUnitOfWork _uow;
        private readonly IDistributedCache _cache;

        public PosTerminalConfigurationService(IUnitOfWork uow, IDistributedCache cache)
        {
            _uow = uow;
            _cache = cache;
        }

        public async Task<IEnumerable<PosTerminalConfigurationDto>> GetAllAsync()
        {
            const string cacheKey = "posterminalconfigurations:all";

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<IEnumerable<PosTerminalConfigurationDto>>(cached)!;

            var terminalConfiguration = await _uow.PosTerminalConfigurations.GetAllByConditionAsync(b =>
                !b.Deleted && b.Is_Active
            );

            var result = terminalConfiguration.Select(x => new PosTerminalConfigurationDto
            {
                Id = x.Id,
                Pos_Terminal_Id = x.Pos_Terminal_Id,
                Config_Key = x.Config_Key,
                Config_Value = x.Config_Value,
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

        public async Task<PosTerminalConfigurationDto?> GetByIdAsync(Guid id)
        {
            var cacheKey = $"posterminalconfigurations:{id}";

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<PosTerminalConfigurationDto>(cached);

            var terminalConfiguration = await _uow.PosTerminalConfigurations.GetByIdAsync(id);
            var dto = terminalConfiguration == null ? null : MapToDto(terminalConfiguration);

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dto), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15) });

            return dto;
        }

        public async Task<PaginatedResponseDto<PosTerminalConfigurationDto>> GetPagedAsync(PosTerminalConfigurationFilterModel filter)
        {
            var cacheKey = PosTerminalConfigurationCacheKeys.Paged(filter.PageNumber, 
                filter.PageSize,
                filter.Pos_Terminal_Id?.ToString() ?? string.Empty,
                filter.Config_Value ?? string.Empty,
                filter.Config_Key ?? string.Empty
                );

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<PaginatedResponseDto<PosTerminalConfigurationDto>>(cached)!;

            var query = _uow.PosTerminalConfigurations.GetQueryable();

            if (filter.Pos_Terminal_Id.HasValue)
                query = query.Where(x => x.Pos_Terminal_Id == filter.Pos_Terminal_Id);

            if (!string.IsNullOrWhiteSpace(filter.Config_Value))
                query = query.Where(x => x.Config_Value.Contains(filter.Config_Value));

            if (!string.IsNullOrWhiteSpace(filter.Config_Key))
                query = query.Where(x => x.Config_Key.Contains(filter.Config_Key));

            query = query.OrderByDescending(x => x.Create_Date);

            var totalRecords = await query.CountAsync();

            var terminalConfiguration = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var result = new PaginatedResponseDto<PosTerminalConfigurationDto>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / filter.PageSize),
                Data = terminalConfiguration.Select(MapToDto).ToList()
            };

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15) });

            return result;
        }
        public async Task<PosTerminalConfigurationDto> CreateAsync(PosTerminalConfigurationCreateDto dto, string userId)
        {
            var terminalConfiguration = new PosTerminalConfiguration
            {
                Id = Guid.NewGuid(),
                Pos_Terminal_Id = dto.Pos_Terminal_Id,
                Config_Key = dto.Config_Key,
                Config_Value = dto.Config_Value,
                BusinessLocation_Id = dto.Business_Location_Id,
                Business_Id = dto.Business_Id,
                Is_Active = true,
                Deleted = false,
                Published = true,
                RecordStatus = Blocks.RecordStatus.Active,
                Create_Date = DateTime.UtcNow,
                Create_User = Guid.Parse(userId)
            };

            await _uow.PosTerminalConfigurations.AddAsync(terminalConfiguration);
            await _uow.SaveAsync();

            // 🔥 CACHE INVALIDATION
            await _cache.RemoveAsync(PosTerminalConfigurationCacheKeys.All);

            return MapToDto(terminalConfiguration);
        }

        public async  Task<PosTerminalConfigurationDto> UpdateAsync(Guid id, PosTerminalConfigurationUpdateDto dto, string userId)
        {
            var entity = await _uow.PosTerminalConfigurations.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Pos Terminal Configuration not found");

            entity.Pos_Terminal_Id = dto.Pos_Terminal_Id;
            entity.Config_Key = dto.Config_Key;
            entity.Config_Value = dto.Config_Value;
            
            entity.Last_Update_Date = DateTime.UtcNow;
            entity.Last_Update_User = Guid.Parse(userId);

            _uow.PosTerminalConfigurations.Update(entity);
            await _uow.SaveAsync();

            // 🔥 CACHE INVALIDATION
            await _cache.RemoveAsync(PosTerminalConfigurationCacheKeys.All);
            await _cache.RemoveAsync(PosTerminalConfigurationCacheKeys.ById(id));

            return MapToDto(entity);
        }

        public async Task<PosTerminalConfigurationDto> DeleteAsync(Guid id, string userId)
        {
            var terminalConfiguration = await _uow.PosTerminalConfigurations.GetByIdAsync(id);
            if (terminalConfiguration == null) return new PosTerminalConfigurationDto();

            terminalConfiguration.Deleted = true;
            terminalConfiguration.Published = false;
            terminalConfiguration.Is_Active = false;
            terminalConfiguration.RecordStatus = Blocks.RecordStatus.Inactive;
            terminalConfiguration.Last_Update_Date = DateTime.UtcNow;
            terminalConfiguration.Last_Update_User = Guid.Parse(userId);

            _uow.PosTerminalConfigurations.Update(terminalConfiguration);
            await _uow.SaveAsync();

            // 🔥 CACHE INVALIDATION
            await _cache.RemoveAsync(PosTerminalConfigurationCacheKeys.All);
            await _cache.RemoveAsync(PosTerminalConfigurationCacheKeys.ById(id));

            return MapToDto(terminalConfiguration);
        }
        private static PosTerminalConfigurationDto MapToDto(PosTerminalConfiguration x) => new()
        {
            Id = x.Id,
            Pos_Terminal_Id = x.Pos_Terminal_Id,
            Config_Key = x.Config_Key,
            Config_Value = x.Config_Value,
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
        };
    }
}
