using Microsoft.EntityFrameworkCore;
using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.PspCurrency;
using NanoDMSAdminService.DTO.PspDocument;
using NanoDMSAdminService.Filters;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Services.Interfaces;
using NanoDMSAdminService.UnitOfWorks;

namespace NanoDMSAdminService.Services.Implementations
{
    public class PspDocumentService : IPspDocumentService
    {
        private readonly IUnitOfWork _uow;

        public PspDocumentService(IUnitOfWork uow)
        {
            _uow = uow;
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

            return MapToDto(pspDocumnet);
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

            return MapToDto(pspDocument);
        }

        public async Task<IEnumerable<PspDocumentDto>> GetAllAsync()
        {
            var pspDocument = await _uow.PspDocuments.GetAllByConditionAsync(b =>
                !b.Deleted && b.Is_Active
            );

            return pspDocument.Select(x => new PspDocumentDto
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
        }

        public async Task<PspDocumentDto?> GetByIdAsync(Guid id)
        {
            var pspDocument = await _uow.PspDocuments.GetByIdAsync(id);
            return pspDocument == null ? null : MapToDto(pspDocument);
        }

        public async Task<PaginatedResponseDto<PspDocumentDto>> GetPagedAsync(PspDocumentFilterModel filter)
        {
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

            return new PaginatedResponseDto<PspDocumentDto>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / filter.PageSize),
                Data = pspDocument.Select(MapToDto).ToList()
            };
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
            return MapToDto(entity);
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
