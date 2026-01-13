using Microsoft.EntityFrameworkCore;
using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.Psp;
using NanoDMSAdminService.DTO.PspCategory;
using NanoDMSAdminService.Filters;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Services.Interfaces;
using NanoDMSAdminService.UnitOfWorks;

namespace NanoDMSAdminService.Services.Implementations
{
    public class PspCategoryService : IPspCategoryService
    {
        private readonly IUnitOfWork _uow;

        public PspCategoryService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PspCategoryDto> CreateAsync(PspCategoryCreateDto dto, string userId)
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

            return MapToDto(pspCategory);
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

            return MapToDto(pspCategory);
        }

        public async Task<IEnumerable<PspCategoryDto>> GetAllAsync()
        {
            var pspCategory = await _uow.PspCategories.GetAllByConditionAsync(b =>
                !b.Deleted && b.Is_Active
            );

            return pspCategory.Select(x => new PspCategoryDto
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
        }

        public async Task<PspCategoryDto?> GetByIdAsync(Guid id)
        {
            var pspCategory = await _uow.PspCategories.GetByIdAsync(id);
            return pspCategory == null ? null : MapToDto(pspCategory);
        }

        public async Task<PaginatedResponseDto<PspCategoryDto>> GetPagedAsync(PspCategoryFilterModel filter)
        {
            var query = _uow.PspCategories.GetQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Name))
                query = query.Where(x => x.Name.Contains(filter.Name));
           
            query = query.OrderByDescending(x => x.Create_Date);

            var totalRecords = await query.CountAsync();

            var pspCategory = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PaginatedResponseDto<PspCategoryDto>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / filter.PageSize),
                Data = pspCategory.Select(MapToDto).ToList()
            };
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
            return MapToDto(entity);
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
