using Microsoft.Extensions.Caching.Distributed;
using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.Bank;
using NanoDMSAdminService.DTO.Currency;
using NanoDMSAdminService.Filters;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Services.Interfaces;
using NanoDMSAdminService.UnitOfWorks;
using NanoDMSSharedLibrary;
using NanoDMSSharedLibrary.CacheKeys;
using System.Text.Json;

namespace NanoDMSAdminService.Services.Implementations
{
    public class BankService : IBankService
    {
        private readonly IUnitOfWork _uow;
        private readonly IDistributedCache _cache;

        public BankService(IUnitOfWork uow, IDistributedCache cache)
        {
            _uow = uow;
            _cache = cache;
        }

        // Get all banks
        public async Task<IEnumerable<BankDto>> GetAllAsync()
        {
            const string cacheKey = "banks:all";

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<IEnumerable<BankDto>>(cached)!;


            var banks = await _uow.Banks.GetAllByConditionAsync(b =>
                !b.Deleted && b.Is_Active
            );

            var result =  banks.Select(b => new BankDto
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
                Business_Id = Guid.Empty,
                BusinessLocation_Id = Guid.Empty,
                Is_Active = b.Is_Active,
                RecordStatus = b.RecordStatus,
            }).ToList();


            await _cache.SetStringAsync(cacheKey,JsonSerializer.Serialize(result),new DistributedCacheEntryOptions{AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)});

            return result;

        }


        // Get by Id
        public async Task<BankDto?> GetByIdAsync(Guid id)
        {
            var cacheKey = $"banks:{id}";

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<BankDto>(cached);

            var bank = await _uow.Banks.GetWithCountryAsync(id);
            if (bank == null) return null;

            var dto = new BankDto
            {
                Id = bank.Id,
                Name = bank.Name,
                Short_Code = bank.Short_Code,
                Short_Name = bank.Short_Name,
                Swift_Code = bank.Swift_Code,
                Country_Id = bank.Country_Id,
                Country_Name = bank.Country?.Name,
                Deleted = bank.Deleted,
                Published = bank.Published,
                Create_Date = bank.Create_Date,
                Create_User = bank.Create_User,
                Last_Update_Date = bank.Last_Update_Date,
                Last_Update_User = bank.Last_Update_User,
                Business_Id = Guid.Empty,
                BusinessLocation_Id = Guid.Empty,
                Is_Active = bank.Is_Active,
                RecordStatus = bank.RecordStatus,
            };

            await _cache.SetStringAsync(cacheKey,JsonSerializer.Serialize(dto),new DistributedCacheEntryOptions{AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)});

            return dto;
        }
        //List 

        public async Task<PaginatedResponseDto<BankDto>> GetPagedAsync(BankFilterModel filter)
        {
            var cacheKey = BankCacheKeys.Paged(filter.PageNumber,filter.PageSize);

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<PaginatedResponseDto<BankDto>>(cached)!;

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
                RecordStatus = b.RecordStatus,
                Published = b.Published,
                Deleted = b.Deleted,
                Create_Date = b.Create_Date,
                Business_Id = Guid.Empty,
                BusinessLocation_Id = Guid.Empty,
                Last_Update_Date = b.Last_Update_Date,
                Last_Update_User = b.Last_Update_User,
            });

            var result =  new PaginatedResponseDto<BankDto>
            {
                TotalRecords = totalRecords,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)filter.PageSize),
                Data = mapped
            };

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15) });

            return result;
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
                Business_Id = Guid.Empty,
                BusinessLocation_Id = Guid.Empty,
                Is_Active = true,
                RecordStatus = Blocks.RecordStatus.Active,
            };

            await _uow.Banks.AddAsync(bank);
            await _uow.SaveAsync();

            // 🔥 CACHE INVALIDATION
            await _cache.RemoveAsync(BankCacheKeys.All);

            return await GetByIdAsync(bank.Id) ?? new BankDto();
        }

        // Update
        public async Task<BankDto> UpdateAsync(Guid id, BankUpdateDto dto,string userId)
        {
            var bank = await _uow.Banks.GetByIdAsync(id);
            if (bank == null) return new BankDto();

            bank.Name = dto.Name;
            bank.Short_Code = dto.Short_Code;
            bank.Short_Name = dto.Short_Name;
            bank.Swift_Code = dto.Swift_Code;
            bank.Country_Id = dto.Country_Id;
            //bank.Business_Id = dto.Business_Id;
            //bank.BusinessLocation_Id = dto. BusinessLocation_Id;

            bank.Deleted = false;
            bank.Published  = true;
            bank.Last_Update_Date = DateTime.UtcNow;
            bank.Last_Update_User = Guid.Parse(userId);
            bank.Is_Active = true;
            bank.RecordStatus = Blocks.RecordStatus.Active;


            _uow.Banks.Update(bank);
            await _uow.SaveAsync();

            // 🔥 CACHE INVALIDATION
            await _cache.RemoveAsync(BankCacheKeys.All);
            await _cache.RemoveAsync(BankCacheKeys.ById(id));

            return await GetByIdAsync(bank.Id) ?? new BankDto();
        }

        // Delete
        public async Task<BankDto> DeleteAsync(Guid id,string userId)
        {
            var bank = await _uow.Banks.GetByIdAsync(id);
            if (bank == null) return new BankDto();

            bank.Deleted = true;
            bank.Published = false;
            bank.Last_Update_Date = DateTime.UtcNow;
            bank.Last_Update_User = Guid.Parse(userId);
            bank.RecordStatus = Blocks.RecordStatus.Inactive;
            bank.Is_Active = false;

            _uow.Banks.Update(bank);
            await _uow.SaveAsync();

            // 🔥 CACHE INVALIDATION
            await _cache.RemoveAsync(BankCacheKeys.All);
            await _cache.RemoveAsync(BankCacheKeys.ById(id));

            return await GetByIdAsync(bank.Id) ?? new BankDto();
        }
    }

}
