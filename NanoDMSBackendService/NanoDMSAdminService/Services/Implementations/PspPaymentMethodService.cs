using Microsoft.EntityFrameworkCore;
using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.PspDocument;
using NanoDMSAdminService.DTO.PspPaymentMethod;
using NanoDMSAdminService.Filters;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Services.Interfaces;
using NanoDMSAdminService.UnitOfWorks;

namespace NanoDMSAdminService.Services.Implementations
{
    public class PspPaymentMethodService : IPspPaymentMethodService
    {
        private readonly IUnitOfWork _uow;

        public PspPaymentMethodService(IUnitOfWork uow)
        {
             _uow = uow;
        }

        public async Task<PspPaymentMethodDto> CreateAsync(PspPaymentMethodCreateDto dto, string userId)
        {
            var pspPaymentMethod = new PspPaymentMethod
            {
                Id = Guid.NewGuid(),
                Psp_Id = dto.Psp_Id,
                Payment_Type = dto.Payment_Type,
                BusinessLocation_Id = dto.Business_Location_Id,
                Business_Id = dto.Business_Id,
                Is_Active = true,
                Deleted = false,
                Published = true,
                RecordStatus = Blocks.RecordStatus.Active,

                Create_Date = DateTime.UtcNow,
                Create_User = Guid.Parse(userId)
            };

            await _uow.PspPaymentMethods.AddAsync(pspPaymentMethod);
            await _uow.SaveAsync();

            return MapToDto(pspPaymentMethod);
        }

        public async Task<PspPaymentMethodDto> DeleteAsync(Guid id, string userId)
        {
            var pspPaymentMethod = await _uow.PspPaymentMethods.GetByIdAsync(id);
            if (pspPaymentMethod == null) return new PspPaymentMethodDto();

            pspPaymentMethod.Deleted = true;
            pspPaymentMethod.Published = false;
            pspPaymentMethod.Is_Active = false;
            pspPaymentMethod.RecordStatus = Blocks.RecordStatus.Inactive;
            pspPaymentMethod.Last_Update_Date = DateTime.UtcNow;
            pspPaymentMethod.Last_Update_User = Guid.Parse(userId);

            _uow.PspPaymentMethods.Update(pspPaymentMethod);
            await _uow.SaveAsync();

            return MapToDto(pspPaymentMethod);
        }

        public async Task<IEnumerable<PspPaymentMethodDto>> GetAllAsync()
        {
            var pspPaymentMethod = await _uow.PspPaymentMethods.GetAllByConditionAsync(b =>
                !b.Deleted && b.Is_Active
            );

            return pspPaymentMethod.Select(x => new PspPaymentMethodDto
            {
                Id = x.Id,
                Psp_Id = x.Psp_Id,
                Psp_Name = x.Psp.Name,
                Payment_Type = x.Payment_Type,
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

        public async Task<PspPaymentMethodDto?> GetByIdAsync(Guid id)
        {
            var pspPaymentMethod = await _uow.PspPaymentMethods.GetByIdAsync(id);
            return pspPaymentMethod == null ? null : MapToDto(pspPaymentMethod);
        }

        public async Task<PaginatedResponseDto<PspPaymentMethodDto>> GetPagedAsync(PspPaymentMethodFilterModel filter)
        {
            var query = _uow.PspPaymentMethods.GetQueryable();

            if (filter.Psp_Id.HasValue)
                query = query.Where(x => x.Psp_Id == filter.Psp_Id);

            if (filter.Payment_Type.HasValue)
                query = query.Where(x => x.Payment_Type == filter.Payment_Type);

            query = query.OrderByDescending(x => x.Create_Date);

            var totalRecords = await query.CountAsync();

            var pspDocument = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PaginatedResponseDto<PspPaymentMethodDto>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / filter.PageSize),
                Data = pspDocument.Select(MapToDto).ToList()
            };
        }

        public async Task<PspPaymentMethodDto> UpdateAsync(Guid id, PspPaymentMethodUpdateDto dto, string userId)
        {
            var entity = await _uow.PspPaymentMethods.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Psp Payment Method not found");

            entity.Psp_Id = dto.Psp_Id;
            entity.Payment_Type = dto.Payment_Type;
            entity.Last_Update_Date = DateTime.UtcNow;
            entity.Last_Update_User = Guid.Parse(userId);

            _uow.PspPaymentMethods.Update(entity);
            await _uow.SaveAsync();
            return MapToDto(entity);
        }

        private static PspPaymentMethodDto MapToDto(PspPaymentMethod x) => new()
        {
            Id = x.Id,
            Psp_Id = x.Psp_Id,
            Psp_Name = x.Psp.Name,
            Payment_Type = x.Payment_Type,
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
