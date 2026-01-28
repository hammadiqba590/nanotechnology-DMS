using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.PspCurrency;
using NanoDMSAdminService.DTO.PspDocument;
using NanoDMSAdminService.DTO.PspPaymentMethod;
using NanoDMSAdminService.Filters;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Services.Interfaces;
using NanoDMSAdminService.UnitOfWorks;
using NanoDMSSharedLibrary.CacheKeys;
using System.Text.Json;

namespace NanoDMSAdminService.Services.Implementations
{
    public class PspDocumentService : IPspDocumentService
    {
        private readonly IUnitOfWork _uow;
        private readonly IDistributedCache _cache;

        public PspDocumentService(IUnitOfWork uow, IDistributedCache cache)
        {
            _uow = uow;
            _cache = cache;
        }

        public async Task<IEnumerable<PspDocumentDto>> GetAllAsync()
        {
            const string cacheKey = "pspdocuments:all";

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<IEnumerable<PspDocumentDto>>(cached)!;

            var pspDocument = await _uow.PspDocuments.GetAllByConditionAsync(b =>
                !b.Deleted && b.Is_Active
            );

            var result = pspDocument.Select(x => new PspDocumentDto
            {
                Id = x.Id,
                Psp_Id = x.Psp_Id,
                Psp_Name = x.Psp.Name,
                Doc_Url = x.Doc_Url,
                Doc_Type = x.Doc_Type,
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

        public async Task<PspDocumentDto?> GetByIdAsync(Guid id)
        {
            var cacheKey = $"pspdocuments:{id}";

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<PspDocumentDto>(cached);

            var pspDocument = await _uow.PspDocuments.GetByIdAsync(id);
            var dto =  pspDocument == null ? null : MapToDto(pspDocument);

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dto), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15) });

            return dto;
        }

        public async Task<PaginatedResponseDto<PspDocumentDto>> GetPagedAsync(PspDocumentFilterModel filter)
        {
            var cacheKey = PspDocumentCacheKeys.Paged(filter.PageNumber, 
                filter.PageSize,
                filter.Psp_Id?.ToString() ?? string.Empty,
                filter.Doc_Type ?? string.Empty
                );

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<PaginatedResponseDto<PspDocumentDto>>(cached)!;

            var query = _uow.PspDocuments.GetQueryable();

            if (filter.Psp_Id.HasValue)
                query = query.Where(x => x.Psp_Id == filter.Psp_Id);

            if (!string.IsNullOrWhiteSpace(filter.Doc_Type))
                query = query.Where(x => x.Doc_Type.Contains(filter.Doc_Type));

            query = query.OrderByDescending(x => x.Create_Date);

            var totalRecords = await query.CountAsync();

            var pspDocument = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var result =  new PaginatedResponseDto<PspDocumentDto>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / filter.PageSize),
                Data = pspDocument.Select(MapToDto).ToList()
            };

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15) });

            return result;
        }
        public async Task<PspDocumentDto> CreateAsync(PspDocumentCreateDto dto, string userId)
        {
            var pspDocumnet = new PspDocument
            {
                Id = Guid.NewGuid(),
                Psp_Id = dto.Psp_Id,
                Doc_Url = dto.Doc_Url,
                Doc_Type = dto.Doc_Type,
                BusinessLocation_Id = dto.Business_Location_Id,
                Business_Id = dto.Business_Id,
                Is_Active = true,
                Deleted = false,
                Published = true,
                RecordStatus = Blocks.RecordStatus.Active,
                Create_Date = DateTime.UtcNow,
                Create_User = Guid.Parse(userId)
            };

            await _uow.PspDocuments.AddAsync(pspDocumnet);
            await _uow.SaveAsync();

            // 🔥 CACHE INVALIDATION
            await _cache.RemoveAsync(PspDocumentCacheKeys.All);

            return MapToDto(pspDocumnet);
        }

        public async Task<PspDocumentDto> UpdateAsync(Guid id, PspDocumentUpdateDto dto, string userId)
        {
            var entity = await _uow.PspDocuments.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Psp Document not found");

            entity.Psp_Id = dto.Psp_Id;
            entity.Doc_Type = dto.Doc_Type;
            entity.Doc_Url = dto.Doc_Url;


            entity.Last_Update_Date = DateTime.UtcNow;
            entity.Last_Update_User = Guid.Parse(userId);

            _uow.PspDocuments.Update(entity);
            await _uow.SaveAsync();

            // 🔥 CACHE INVALIDATION
            await _cache.RemoveAsync(PspDocumentCacheKeys.All);
            await _cache.RemoveAsync(PspDocumentCacheKeys.ById(id));

            return MapToDto(entity);
        }
        public async Task<PspDocumentDto> DeleteAsync(Guid id, string userId)
        {
            var pspDocument = await _uow.PspDocuments.GetByIdAsync(id);
            if (pspDocument == null) return new PspDocumentDto();

            pspDocument.Deleted = true;
            pspDocument.Published = false;
            pspDocument.Is_Active = false;
            pspDocument.RecordStatus = Blocks.RecordStatus.Inactive;
            pspDocument.Last_Update_Date = DateTime.UtcNow;
            pspDocument.Last_Update_User = Guid.Parse(userId);

            _uow.PspDocuments.Update(pspDocument);
            await _uow.SaveAsync();

            // 🔥 CACHE INVALIDATION
            await _cache.RemoveAsync(PspDocumentCacheKeys.All);
            await _cache.RemoveAsync(PspDocumentCacheKeys.ById(id));

            return MapToDto(pspDocument);
        }

        private static PspDocumentDto MapToDto(PspDocument x) => new()
        {
            Id = x.Id,
            Psp_Id = x.Psp_Id,
            Psp_Name = x.Psp.Name,
            Doc_Type = x.Doc_Type,
            Doc_Url = x.Doc_Url,
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
