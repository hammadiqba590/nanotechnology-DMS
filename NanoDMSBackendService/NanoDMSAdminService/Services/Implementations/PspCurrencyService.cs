using Microsoft.EntityFrameworkCore;
using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.PspCategory;
using NanoDMSAdminService.DTO.PspCurrency;
using NanoDMSAdminService.Filters;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Services.Interfaces;
using NanoDMSAdminService.UnitOfWorks;

namespace NanoDMSAdminService.Services.Implementations
{
    public class PspCurrencyService : IPspCurrencyService
    {
        private readonly IUnitOfWork _uow;

        public PspCurrencyService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PspCurrencyDto> CreateAsync(PspCurrencyCreateDto dto, string userId)
        {
            var pspCurrency = new PspCurrency
            {
                Id = Guid.NewGuid(),
                Psp_Id = dto.Psp_Id,
                Currency_Id = dto.Currency_Id,
                BusinessLocation_Id = dto.Business_Location_Id,
                Business_Id = dto.Business_Id,
                Is_Active = true,
                Deleted = false,
                Published = true,
                RecordStatus = Blocks.RecordStatus.Active,

                Create_Date = DateTime.UtcNow,
                Create_User = Guid.Parse(userId)
            };

            await _uow.PspCurrencies.AddAsync(pspCurrency);
            await _uow.SaveAsync();

            return MapToDto(pspCurrency);
        }

        public async Task<PspCurrencyDto> DeleteAsync(Guid id, string userId)
        {
            var pspCurrency = await _uow.PspCurrencies.GetByIdAsync(id);
            if (pspCurrency == null) return new PspCurrencyDto();

            pspCurrency.Deleted = true;
            pspCurrency.Published = false;
            pspCurrency.Is_Active = false;
            pspCurrency.RecordStatus = Blocks.RecordStatus.Inactive;
            pspCurrency.Last_Update_Date = DateTime.UtcNow;
            pspCurrency.Last_Update_User = Guid.Parse(userId);

            _uow.PspCurrencies.Update(pspCurrency);
            await _uow.SaveAsync();

            return MapToDto(pspCurrency);
        }

        public async Task<IEnumerable<PspCurrencyDto>> GetAllAsync()
        {
            var pspCurrency = await _uow.PspCurrencies.GetAllByConditionAsync(b =>
                !b.Deleted && b.Is_Active
            );

            return pspCurrency.Select(x => new PspCurrencyDto
            {
                Id = x.Id,
                Psp_Id = x.Psp_Id,
                Currency_Id = x.Currency_Id,
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

        public async Task<PspCurrencyDto?> GetByIdAsync(Guid id)
        {
            var pspCurrency = await _uow.PspCurrencies.GetByIdAsync(id);
            return pspCurrency == null ? null : MapToDto(pspCurrency);
        }

        public async Task<PaginatedResponseDto<PspCurrencyDto>> GetPagedAsync(PspCurrencyFilterModel filter)
        {
            var query = _uow.PspCurrencies.GetQueryable();

            if (filter.Psp_Id.HasValue)
                query = query.Where(x => x.Psp_Id == filter.Psp_Id);

            if (filter.Currency_Id.HasValue)
                query = query.Where(x => x.Currency_Id == filter.Currency_Id);

            query = query.OrderByDescending(x => x.Create_Date);

            var totalRecords = await query.CountAsync();

            var pspCurrency = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PaginatedResponseDto<PspCurrencyDto>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / filter.PageSize),
                Data = pspCurrency.Select(MapToDto).ToList()
            };
        }

        public async Task<PspCurrencyDto> UpdateAsync(Guid id, PspCurrencyUpdateDto dto, string userId)
        {
            var entity = await _uow.PspCurrencies.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Psp Currency not found");

            entity.Psp_Id = dto.Psp_Id;
            entity.Currency_Id = dto.Currency_Id;
            

            entity.Last_Update_Date = DateTime.UtcNow;
            entity.Last_Update_User = Guid.Parse(userId);

            _uow.PspCurrencies.Update(entity);
            await _uow.SaveAsync();
            return MapToDto(entity);
        }

        private static PspCurrencyDto MapToDto(PspCurrency x) => new()
        {
            Id = x.Id,
            Psp_Id = x.Psp_Id,
            Psp_Name = x.Psp.Name,
            Currency_Id = x.Currency_Id,
            Currency_Name = x.Currency.Name,
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
