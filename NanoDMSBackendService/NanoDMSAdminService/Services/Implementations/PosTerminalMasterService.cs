using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.CardType;
using NanoDMSAdminService.DTO.DiscountRule;
using NanoDMSAdminService.DTO.PosTerminalMaster;
using NanoDMSAdminService.Filters;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Services.Interfaces;
using NanoDMSAdminService.UnitOfWorks;
using NanoDMSSharedLibrary.CacheKeys;
using System.Text.Json;

namespace NanoDMSAdminService.Services.Implementations
{
    public class PosTerminalMasterService : IPosTerminalMasterService
    {
        private readonly IUnitOfWork _uow;
        private readonly IDistributedCache _cache;

        public PosTerminalMasterService(IUnitOfWork uow , IDistributedCache cache)
        {
            _uow = uow;
            _cache = cache;
        }

        public async Task<IEnumerable<PosTerminalMasterDto>> GetAllAsync()
        {
            const string cacheKey = "posterminalmasters:all";

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<IEnumerable<PosTerminalMasterDto>>(cached)!;

            var terminal = await _uow.PosTerminalMasters.GetAllByConditionAsync(b =>
                !b.Deleted && b.Is_Active
            );

            var result = terminal.Select(x => new PosTerminalMasterDto
            {
                Id = x.Id,
                Serial_Number = x.Serial_Number,
                Terminal_Code = x.Terminal_Code,
                Company = x.Company,
                Model_Number = x.Model_Number,
                Software_Version = x.Software_Version,
                Description = x.Description,
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

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)});

            return result;
        }

        public async Task<PosTerminalMasterDto?> GetByIdAsync(Guid id)
        {
            var cacheKey = $"posterminalmasters:{id}";

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<PosTerminalMasterDto>(cached);

            var terminal = await _uow.PosTerminalMasters.GetByIdAsync(id);
            var dto = terminal == null ? null : MapToDto(terminal);

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dto), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15) });

            return dto;

        }

        public async Task<PaginatedResponseDto<PosTerminalMasterDto>> GetPagedAsync(PosTerminalMasterFilterModel filter)
        {
            var cacheKey = PosTerminalMasterCacheKeys.Paged(filter.PageNumber, 
                filter.PageSize,
                filter.Serial_Number ?? string.Empty,
                filter.Terminal_Code ?? string.Empty,
                filter.Company ?? string.Empty,
                filter.Model_Number ?? string.Empty
                );

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<PaginatedResponseDto<PosTerminalMasterDto>>(cached)!;

            var query = _uow.PosTerminalMasters.GetQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Serial_Number))
                query = query.Where(x => x.Serial_Number.Contains(filter.Serial_Number));

            if(!string.IsNullOrWhiteSpace(filter.Terminal_Code))
                query = query.Where(x => x.Terminal_Code!.Contains(filter.Terminal_Code));

            if (!string.IsNullOrWhiteSpace(filter.Company))
                query = query.Where(x => x.Company!.Contains(filter.Company));

            if (!string.IsNullOrWhiteSpace(filter.Model_Number))
                query = query.Where(x => x.Model_Number!.Contains(filter.Model_Number));


            query = query.OrderByDescending(x => x.Create_Date);

            var totalRecords = await query.CountAsync();

            var terminal = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var result = new PaginatedResponseDto<PosTerminalMasterDto>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / filter.PageSize),
                Data = terminal.Select(MapToDto).ToList()
            };

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15) });

            return result;
        }

        public async Task<PosTerminalMasterDto> CreateAsync(PosTerminalMasterCreateDto dto, string userId)
        {
            var terminal = new PosTerminalMaster
            {
                Id = Guid.NewGuid(),
                Serial_Number = dto.Serial_Number,
                Terminal_Code = dto.Terminal_Code,
                Company = dto.Company,
                Model_Number = dto.Model_Number,
                Software_Version = dto.Software_Version,
                Description = dto.Description,
                BusinessLocation_Id = dto.Business_Location_Id,
                Business_Id = dto.Business_Id,
                Is_Active = true,
                Deleted = false,
                Published = true,
                RecordStatus = Blocks.RecordStatus.Active,

                Create_Date = DateTime.UtcNow,
                Create_User = Guid.Parse(userId)
            };

            await _uow.PosTerminalMasters.AddAsync(terminal);
            await _uow.SaveAsync();

            // 🔥 CACHE INVALIDATION
            await _cache.RemoveAsync(PosTerminalMasterCacheKeys.All);

            return MapToDto(terminal);
        }

        public async Task<PosTerminalMasterDto> UpdateAsync(Guid id, PosTerminalMasterUpdateDto dto, string userId)
        {
            var entity = await _uow.PosTerminalMasters.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Pos Terminal not found");

            entity.Serial_Number = dto.Serial_Number;
            entity.Model_Number = dto.Model_Number;
            entity.Software_Version = dto.Software_Version;
            entity.Company = dto.Company;
            entity.Terminal_Code = dto.Terminal_Code;
            entity.Description = dto.Description;

            entity.Last_Update_Date = DateTime.UtcNow;
            entity.Last_Update_User = Guid.Parse(userId);

            _uow.PosTerminalMasters.Update(entity);
            await _uow.SaveAsync();

            // 🔥 CACHE INVALIDATION
            await _cache.RemoveAsync(PosTerminalMasterCacheKeys.All);
            await _cache.RemoveAsync(PosTerminalMasterCacheKeys.ById(id));

            return MapToDto(entity);
        }
        public async Task<PosTerminalMasterDto> DeleteAsync(Guid id, string userId)
        {
            var terminal = await _uow.PosTerminalMasters.GetByIdAsync(id);
            if (terminal == null) return new PosTerminalMasterDto();

            terminal.Deleted = true;
            terminal.Published = false;
            terminal.Is_Active = false;
            terminal.RecordStatus = Blocks.RecordStatus.Inactive;
            terminal.Last_Update_Date = DateTime.UtcNow;
            terminal.Last_Update_User = Guid.Parse(userId);

            _uow.PosTerminalMasters.Update(terminal);
            await _uow.SaveAsync();

            // 🔥 CACHE INVALIDATION
            await _cache.RemoveAsync(PosTerminalMasterCacheKeys.All);
            await _cache.RemoveAsync(PosTerminalMasterCacheKeys.ById(id));

            return MapToDto(terminal);
        }
        private static PosTerminalMasterDto MapToDto(PosTerminalMaster x) => new()
        {
            Id = x.Id,
            Serial_Number = x.Serial_Number,
            Terminal_Code = x.Terminal_Code,
            Company = x.Company,
            Model_Number = x.Model_Number,
            Software_Version = x.Software_Version,
            Description = x.Description,
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
