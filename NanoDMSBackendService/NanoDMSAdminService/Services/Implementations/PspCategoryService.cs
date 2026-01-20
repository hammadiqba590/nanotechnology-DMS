using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.Psp;
using NanoDMSAdminService.DTO.PspCategory;
using NanoDMSAdminService.DTO.PspCurrency;
using NanoDMSAdminService.Filters;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Services.Interfaces;
using NanoDMSAdminService.UnitOfWorks;
using NanoDMSSharedLibrary.CacheKeys;
using System.Text.Json;

namespace NanoDMSAdminService.Services.Implementations
{
    public class PspCategoryService : IPspCategoryService
    {
        private readonly IUnitOfWork _uow;
        private readonly IDistributedCache _cache;

        public PspCategoryService(IUnitOfWork uow, IDistributedCache cache)
        {
            _uow = uow;
            _cache = cache;
        }

        public async Task<IEnumerable<PspCategoryDto>> GetAllAsync()
        {
            const string cacheKey = "pspcategories:all";

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<IEnumerable<PspCategoryDto>>(cached)!;

            var pspCategory = await _uow.PspCategories.GetAllByConditionAsync(b =>
                !b.Deleted && b.Is_Active
            );

            var result = pspCategory.Select(x => new PspCategoryDto
            {
                Id = x.Id,
                Name = x.Name,
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

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) });

            return result;
        }

        public async Task<PspCategoryDto?> GetByIdAsync(Guid id)
        {
            var cacheKey = $"pspcategories:{id}";

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<PspCategoryDto>(cached);

            var pspCategory = await _uow.PspCategories.GetByIdAsync(id);
            var dto = pspCategory == null ? null : MapToDto(pspCategory);

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dto), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15) });

            return dto;
        }

        public async Task<PaginatedResponseDto<PspCategoryDto>> GetPagedAsync(PspCategoryFilterModel filter)
        {
            var cacheKey = PspCategoryCacheKeys.Paged(filter.PageNumber, filter.PageSize);

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<PaginatedResponseDto<PspCategoryDto>>(cached)!;

            var query = _uow.PspCategories.GetQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Name))
                query = query.Where(x => x.Name.Contains(filter.Name));
           
            query = query.OrderByDescending(x => x.Create_Date);

            var totalRecords = await query.CountAsync();

            var pspCategory = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var result = new PaginatedResponseDto<PspCategoryDto>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / filter.PageSize),
                Data = pspCategory.Select(MapToDto).ToList()
            };

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15) });

            return result;
        }
        public async Task<PspCategoryDto> CreateAsync(PspCategoryCreateDto dto, string userId)
        {
            try
            {
                var pspCategory = new PspCategory
                {
                    Id = Guid.NewGuid(),
                    Name = dto.Name,
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

                await _uow.PspCategories.AddAsync(pspCategory);
                await _uow.SaveAsync();

                // 🔥 CACHE INVALIDATION
                await _cache.RemoveAsync(PspCategoryCacheKeys.All);

                return MapToDto(pspCategory);
            }
            catch (Exception ex)
            {
                // Fixes CS0029 and CS8602: Always throw an Exception, and handle possible nulls
                if (ex.InnerException != null)
                {
                    throw new Exception(ex.InnerException.Message, ex.InnerException);
                }
                throw;
            }
        }

        public async Task<PspCategoryDto> UpdateAsync(Guid id, PspCategoryUpdateDto dto, string userId)
        {
            var entity = await _uow.PspCategories.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Psp Category not found");

            entity.Name = dto.Name;
            entity.Description = dto.Description;

            entity.Last_Update_Date = DateTime.UtcNow;
            entity.Last_Update_User = Guid.Parse(userId);

            _uow.PspCategories.Update(entity);
            await _uow.SaveAsync();

            // 🔥 CACHE INVALIDATION
            await _cache.RemoveAsync(PspCategoryCacheKeys.All);
            await _cache.RemoveAsync(PspCategoryCacheKeys.ById(id));

            return MapToDto(entity);
        }
        public async Task<PspCategoryDto> DeleteAsync(Guid id, string userId)
        {
            var pspCategory = await _uow.PspCategories.GetByIdAsync(id);
            if (pspCategory == null) return new PspCategoryDto();

            pspCategory.Deleted = true;
            pspCategory.Published = false;
            pspCategory.Is_Active = false;
            pspCategory.RecordStatus = Blocks.RecordStatus.Inactive;
            pspCategory.Last_Update_Date = DateTime.UtcNow;
            pspCategory.Last_Update_User = Guid.Parse(userId);

            _uow.PspCategories.Update(pspCategory);
            await _uow.SaveAsync();

            // 🔥 CACHE INVALIDATION
            await _cache.RemoveAsync(PspCategoryCacheKeys.All);
            await _cache.RemoveAsync(PspCategoryCacheKeys.ById(id));

            return MapToDto(pspCategory);
        }
        private static PspCategoryDto MapToDto(PspCategory x) => new()
        {
            Id = x.Id,
            Name = x.Name,
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
