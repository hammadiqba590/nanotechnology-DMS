using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.Bank;
using NanoDMSAdminService.Filters;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.UnitOfWorks;

namespace NanoDMSAdminService.Services.Implementations
{
    public class BankService : IBankService
    {
        private readonly IUnitOfWork _uow;

        public BankService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // Get all banks
        public async Task<IEnumerable<BankDto>> GetAllAsync()
        {
            var banks = await _uow.Banks.GetAllByConditionAsync(b =>
                !b.Deleted && b.Is_Active
            );

            return banks.Select(b => new BankDto
            {
                Id = b.Id,
                Name = b.Name,
                Short_Code = b.Short_Code,
                Short_Name = b.Short_Name,
                Swift_Code = b.Swift_Code,
                Country_Id = b.Country_Id,
                Country_Name = b.Country?.Name ?? string.Empty,
                Deleted = b.Deleted,
                Published = b.Published,
                Create_Date = b.Create_Date,
                Create_User = b.Create_User,
                Last_Update_Date = b.Last_Update_Date,
                Last_Update_User = b.Last_Update_User,
                Business_Id = b.Business_Id,
                BusinessLocation_Id = b.BusinessLocation_Id,
                Is_Active = b.Is_Active,
                Status = b.Status,
            });
        }


        // Get by Id
        public async Task<BankDto?> GetByIdAsync(Guid id)
        {
            var bank = await _uow.Banks.GetWithCountryAsync(id);
            if (bank == null) return null;

            return new BankDto
            {
                Id = bank.Id,
                Name = bank.Name,
                Short_Code = bank.Short_Code,
                Short_Name = bank.Short_Name,
                Swift_Code = bank.Swift_Code,
                Country_Id = bank.Country_Id,
                Deleted = bank.Deleted,
                Published = bank.Published,
                Create_Date = bank.Create_Date,
                Create_User = bank.Create_User,
                Last_Update_Date = bank.Last_Update_Date,
                Last_Update_User = bank.Last_Update_User,
                Business_Id = bank.Business_Id,
                BusinessLocation_Id = bank.BusinessLocation_Id,
                Is_Active = bank.Is_Active,
                Status = bank.Status,
            };
        }
        //List 

        public async Task<PaginatedResponseDto<BankDto>> GetPagedAsync(BankFilterModel filter)
        {
            var (banks, totalRecords) = await _uow.Banks.GetPagedAsync(filter);

            if (!banks.Any())
                return new PaginatedResponseDto<BankDto>();

            var mapped = banks.Select(b => new BankDto
            {
                Id = b.Id,
                Name = b.Name,
                Short_Code = b.Short_Code,
                Short_Name = b.Short_Name,
                Swift_Code = b.Swift_Code,
                Country_Id = b.Country_Id,
                Country_Name = b.Country?.Name ?? "",
                Is_Active = b.Is_Active,
                Status = b.Status,
                Published = b.Published,
                Deleted = b.Deleted,
                Create_Date = b.Create_Date,
                Business_Id = b.Business_Id,
                BusinessLocation_Id = b.BusinessLocation_Id,
                Last_Update_Date = b.Last_Update_Date,
                Last_Update_User = b.Last_Update_User,
            });

            return new PaginatedResponseDto<BankDto>
            {
                TotalRecords = totalRecords,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)filter.PageSize),
                Data = mapped
            };
        }


        // Create
        public async Task<BankDto> CreateAsync(BankCreateDto dto,string userId)
        {
            var bank = new Bank
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Short_Code = dto.Short_Code,
                Swift_Code = dto.Swift_Code,
                Short_Name = dto.Short_Name,
                Country_Id = dto.Country_Id,
                Deleted = false,
                Published = true,
                Create_Date = DateTime.UtcNow,
                Create_User = Guid.Parse(userId),
                Business_Id = dto.Business_Id,
                BusinessLocation_Id = dto.BusinessLocation_Id,
                Is_Active = true,
                Status = Blocks.RecordStatus.Active,
            };

            await _uow.Banks.AddAsync(bank);
            await _uow.SaveAsync();

            return new BankDto
            {
                Id = bank.Id,
                Name = bank.Name,
                Short_Code = bank.Short_Code,
                Short_Name = bank.Short_Name,
                Swift_Code = bank.Swift_Code,
                Country_Id = bank.Country_Id,
                Deleted = bank.Deleted,
                Published = bank.Published,
                Create_Date = bank.Create_Date,
                Create_User = bank.Create_User,
                Last_Update_Date = bank.Last_Update_Date,
                Last_Update_User = bank.Last_Update_User,
                Business_Id = bank.Business_Id,
                BusinessLocation_Id = bank.BusinessLocation_Id,
                Is_Active = bank.Is_Active,
                Status = bank.Status,
            };
        }

        // Update
        public async Task<BankDto> UpdateAsync(Guid id, BankUpdateDto dto,string userId)
        {
            var bank = await _uow.Banks.GetByIdAsync(id);
            if (bank == null) return null;

            bank.Name = dto.Name;
            bank.Short_Code = dto.Short_Code;
            bank.Short_Name = dto.Short_Name;
            bank.Swift_Code = dto.Swift_Code;
            bank.Country_Id = dto.Country_Id;
            bank.Business_Id = dto.Business_Id;
            bank.BusinessLocation_Id = dto. BusinessLocation_Id;

            bank.Deleted = false;
            bank.Published  = true;
            bank.Last_Update_Date = DateTime.UtcNow;
            bank.Last_Update_User = Guid.Parse(userId);
            bank.Is_Active = true;
            bank.Status = Blocks.RecordStatus.Active;


            _uow.Banks.Update(bank);
            await _uow.SaveAsync();
           return new BankDto
            {
                Id = bank.Id,
                Name = bank.Name,
                Short_Code = bank.Short_Code,
                Short_Name = bank.Short_Name,
                Swift_Code = bank.Swift_Code,
                Country_Id = bank.Country_Id,
                Deleted = bank.Deleted,
                Published = bank.Published,
                Create_Date = bank.Create_Date,
                Create_User = bank.Create_User,
                Last_Update_Date = bank.Last_Update_Date,
                Last_Update_User = bank.Last_Update_User,
                Business_Id = bank.Business_Id,
                BusinessLocation_Id = bank.BusinessLocation_Id,
                Is_Active = bank.Is_Active,
                Status = bank.Status,
            };
        }

        // Delete
        public async Task<BankDto> DeleteAsync(Guid id,string userId)
        {
            var bank = await _uow.Banks.GetByIdAsync(id);
            if (bank == null) return null;

            bank.Deleted = true;
            bank.Published = false;
            bank.Last_Update_Date = DateTime.UtcNow;
            bank.Last_Update_User = Guid.Parse(userId);
            bank.Status = Blocks.RecordStatus.Inactive;
            bank.Is_Active = false;

            _uow.Banks.Update(bank);
            await _uow.SaveAsync();
            return new BankDto
            {
                Id = bank.Id,
                Name = bank.Name,
                Short_Code = bank.Short_Code,
                Short_Name = bank.Short_Name,
                Swift_Code = bank.Swift_Code,
                Country_Id = bank.Country_Id,
                Deleted = bank.Deleted,
                Published = bank.Published,
                Create_Date = bank.Create_Date,
                Create_User = bank.Create_User,
                Last_Update_Date = bank.Last_Update_Date,
                Last_Update_User = bank.Last_Update_User,
                Business_Id = bank.Business_Id,
                BusinessLocation_Id = bank.BusinessLocation_Id,
                Is_Active = bank.Is_Active,
                Status = bank.Status,
            };
        }
    }

}
