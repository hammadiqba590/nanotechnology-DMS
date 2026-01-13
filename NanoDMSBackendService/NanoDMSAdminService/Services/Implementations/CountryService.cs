using Microsoft.EntityFrameworkCore;
using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.Bank;
using NanoDMSAdminService.DTO.Country;
using NanoDMSAdminService.Filters;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.UnitOfWorks;

namespace NanoDMSAdminService.Services.Implementations
{
    public class CountryService : ICountryService
    {
        private readonly IUnitOfWork _uow;

        public CountryService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PaginatedResponseDto<CountryDto>> GetPagedAsync(CountryFilterModel filter)
        {
            var query = _uow.Countries.GetQueryable()
                .Where(x => !x.Deleted);

            if (!string.IsNullOrEmpty(filter.Name))
                query = query.Where(x => x.Name.Contains(filter.Name));

            if (!string.IsNullOrEmpty(filter.Iso2))
                query = query.Where(x => x.Iso2 == filter.Iso2);

            var totalRecords = await query.CountAsync();

            var data = await query
                .OrderByDescending(x => x.Create_Date)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(x => new CountryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Iso2 = x.Iso2,
                    Iso3 = x.Iso3,
                    Currency_Code = x.Currency_Code,
                    Phone_Code = x.Phone_Code,
                    Flag_Emoji = x.Flag_Emoji,
                    Deleted = x.Deleted,
                    Published = x.Published,
                    Is_Active = x.Is_Active,
                    Create_Date = x.Create_Date,
                    Last_Update_Date = x.Last_Update_Date
                })
                .ToListAsync();

            return new PaginatedResponseDto<CountryDto>
            {
                TotalRecords = totalRecords,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / filter.PageSize),
                Data = data
            };
        }

        public async Task<IEnumerable<CountryDto>> GetAllAsync()
        {
            var country = await _uow.Countries.GetAllByConditionAsync(b =>
                !b.Deleted && b.Is_Active
            );

            return country.Select(b => new CountryDto
            {
                Id = b.Id,
                Name = b.Name,
                Iso2 = b.Iso2,
                Iso3 = b.Iso3,
                Numeric_Code = b.Numeric_Code,
                Phone_Code = b.Phone_Code,
                Currency_Code = b.Currency_Code,
                Currency_Symbol = b.Currency_Symbol,
                Flag_Emoji = b.Flag_Emoji,
                Time_Zone = b.Time_Zone,
                Deleted = b.Deleted,
                Published = b.Published,
                Create_Date = b.Create_Date,
                Create_User = b.Create_User,
                Last_Update_Date = b.Last_Update_Date,
                Last_Update_User = b.Last_Update_User,
                Business_Id = b.Business_Id,
                BusinessLocation_Id = b.BusinessLocation_Id,
                Is_Active = b.Is_Active,
                RecordStatus = b.RecordStatus,
            });
        }
        public async Task<CountryDto?> GetByIdAsync(Guid id)
        {
            var country = await _uow.Countries.GetByIdAsync(id);
            if (country == null) return null;

            return new CountryDto
            {
                Id = country.Id,
                Name = country.Name,
                Iso2 = country.Iso2,
                Iso3 = country.Iso3,
                Currency_Code = country.Currency_Code,
                Currency_Symbol = country.Currency_Symbol,
                Time_Zone = country.Time_Zone,
                Phone_Code = country.Phone_Code,
                Flag_Emoji = country.Flag_Emoji,
                Deleted = country.Deleted,
                Published = country.Published,
                Is_Active = country.Is_Active,
                BusinessLocation_Id= country.BusinessLocation_Id,
                Business_Id = country.BusinessLocation_Id,
                Create_Date = country.Create_Date,
                Create_User = country.Create_User,
                Last_Update_User = country.Last_Update_User,
                Last_Update_Date = country.Last_Update_Date
            };
        }

        public async Task<CountryDto> CreateAsync(CountryCreateDto dto, string userId)
        {
            var country = new Country
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Iso2 = dto.Iso2,
                Iso3 = dto.Iso3,
                Numeric_Code = dto.Numeric_Code,
                Phone_Code = dto.Phone_Code,
                Currency_Code = dto.Currency_Code,
                Currency_Symbol = dto.Currency_Symbol,
                Flag_Emoji = dto.Flag_Emoji,
                Time_Zone = dto.Time_Zone,

                Create_Date = DateTime.UtcNow,
                Create_User = Guid.Parse(userId),
                Published = true,
                Deleted = false,
                Is_Active = true,
                RecordStatus = Blocks.RecordStatus.Active,
            };

            await _uow.Countries.AddAsync(country);
            await _uow.SaveAsync();

            return await GetByIdAsync(country.Id) ?? new CountryDto();
        }

        public async Task<CountryDto> UpdateAsync(Guid id, CountryUpdateDto dto, string userId)
        {
            var country = await _uow.Countries.GetByIdAsync(id);
            if (country == null) return new CountryDto();

            country.Name = dto.Name;
            country.Iso2 = dto.Iso2;
            country.Iso3 = dto.Iso3;
            country.Numeric_Code = dto.Numeric_Code;
            country.Phone_Code = dto.Phone_Code;
            country.Currency_Code = dto.Currency_Code;
            country.Currency_Symbol = dto.Currency_Symbol;
            country.Flag_Emoji = dto.Flag_Emoji;
            country.Time_Zone = dto.Time_Zone; 

            country.Last_Update_Date = DateTime.UtcNow;
            country.Last_Update_User = Guid.Parse(userId);

            _uow.Countries.Update(country);
            await _uow.SaveAsync();

            return await GetByIdAsync(id) ?? new CountryDto();
        }

        public async Task<CountryDto> DeleteAsync(Guid id, string userId)
        {
            var country = await _uow.Countries.GetByIdAsync(id);
            if (country == null) return new CountryDto();

            country.Deleted = true;
            country.Is_Active = false;
            country.Published = false;
            country.RecordStatus = Blocks.RecordStatus.Inactive;
            country.Last_Update_User = Guid.Parse(userId);
            country.Last_Update_Date = DateTime.UtcNow;

            _uow.Countries.Update(country);
            await _uow.SaveAsync();

            return await GetByIdAsync(id) ?? new CountryDto();
        }
    }

}
