using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.PosTerminalMaster;
using NanoDMSAdminService.DTO.Psp;
using NanoDMSAdminService.Filters;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Services.Interfaces;
using NanoDMSAdminService.UnitOfWorks;
using NanoDMSSharedLibrary.CacheKeys;
using System.Text.Json;

namespace NanoDMSAdminService.Services.Implementations
{
    public class PspService : IPspService
    {
        private readonly IUnitOfWork _uow;
        private readonly IDistributedCache _cache;

        public PspService(IUnitOfWork uow, IDistributedCache cache)
        {
            _uow = uow;
            _cache = cache;
        }

        public async Task<IEnumerable<PspDto>> GetAllAsync()
        {
            const string cacheKey = "psps:all";

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<IEnumerable<PspDto>>(cached)!;

            var psp = await _uow.Psps.GetAllByConditionAsync(b =>
                !b.Deleted && b.Is_Active
            );

            var result = psp.Select(x => new PspDto
            {
                Id = x.Id,
                Name = x.Name,
                Short_Name = x.Short_Name,
                Code = x.Code,
                Psp_Category_Id = x.Psp_Category_Id,
                Psp_Category_Name = x.PspCategory.Name,
                Country_Id = x.Country_Id,
                Country_Name = x.Country?.Name,
                Currency_Code = x.Currency_Code,
                Currency_Symbol = x.Currency_Symbol,
                Registration_Number = x.Registration_Number,
                Reg_Doc_Url = x.Reg_Doc_Url,
                Compliance_Status = x.Compliance_Status,
                Website = x.Website,
                Contact_Email = x.Contact_Email,
                Contact_Phone = x.Contact_Phone,
                Api_Endpoint = x.Api_Endpoint,
                Sandbox_Endpoint = x.Sandbox_Endpoint,
                Webhook_Url = x.Webhook_Url,
                Api_Key = x.Api_Key,
                Documentation_Url = x.Documentation_Url,
                Integration_Type = x.Integration_Type,
                Supported_Payment_Methods = x.Supported_Payment_Methods,
                Supported_Currencies = x.Supported_Currencies,
                Settlement_Frequency = x.Settlement_Frequency,
                Transaction_Limit = x.Transaction_Limit,
                Daily_Volume_Limit = x.Daily_Volume_Limit,
                Risk_Score = x.Risk_Score,
                Requires_Kyc = x.Requires_Kyc,
                Onboarded_By = x.Onboarded_By,
                Onboarded_At = x.Onboarded_At,
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

        public async Task<PspDto?> GetByIdAsync(Guid id)
        {
            var cacheKey = $"psps:{id}";

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<PspDto>(cached);

            var psp = await _uow.Psps.GetByIdAsync(id);
            var dto = psp == null ? null : MapToDto(psp);

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dto), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15) });

            return dto;
        }

        public async Task<PaginatedResponseDto<PspDto>> GetPagedAsync(PspFilterModel filter)
        {
            var cacheKey = PspCacheKeys.Paged(filter.PageNumber, filter.PageSize);

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<PaginatedResponseDto<PspDto>>(cached)!;

            var query = _uow.Psps.GetQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Name))
                query = query.Where(x => x.Name.Contains(filter.Name));

            if (!string.IsNullOrWhiteSpace(filter.Short_Name))
                query = query.Where(x => x.Short_Name!.Contains(filter.Short_Name));

            if (!string.IsNullOrWhiteSpace(filter.Code))
                query = query.Where(x => x.Code!.Contains(filter.Code));

            if (!string.IsNullOrWhiteSpace(filter.Currency_Code))
                query = query.Where(x => x.Currency_Code!.Contains(filter.Currency_Code));

            if (!string.IsNullOrWhiteSpace(filter.Registration_Number))
                query = query.Where(x => x.Registration_Number!.Contains(filter.Registration_Number));

            if (!string.IsNullOrWhiteSpace(filter.Contact_Email))
                query = query.Where(x => x.Contact_Email!.Contains(filter.Contact_Email));

            if (!string.IsNullOrWhiteSpace(filter.Contact_Phone))
                query = query.Where(x => x.Contact_Phone!.Contains(filter.Contact_Phone));

            if (!string.IsNullOrWhiteSpace(filter.Api_Key))
                query = query.Where(x => x.Api_Key!.Contains(filter.Api_Key));

            if (filter.Integration_Type.HasValue)
                query = query.Where(x => x.Integration_Type == filter.Integration_Type);

            if (filter.Compliance_Status.HasValue)
                query = query.Where(x => x.Compliance_Status == filter.Compliance_Status);

            if (filter.Settlement_Frequency.HasValue)
                query = query.Where(x => x.Settlement_Frequency == filter.Settlement_Frequency);

            if (filter.Requires_Kyc.Equals(filter.Requires_Kyc))
                query = query.Where(x => x.Requires_Kyc == filter.Requires_Kyc);


            query = query.OrderByDescending(x => x.Create_Date);

            var totalRecords = await query.CountAsync();

            var psp = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var result = new PaginatedResponseDto<PspDto>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / filter.PageSize),
                Data = psp.Select(MapToDto).ToList()
            };

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15) });

            return result;
        }

        public async Task<PspDto> CreateAsync(PspCreateDto dto, string userId)
        {
            var psp = new Psp
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Short_Name = dto.Short_Name,
                Code = dto.Code,
                Psp_Category_Id = dto.Psp_Category_Id,
                Country_Id = dto.Country_Id,
                Currency_Code = dto.Currency_Code,
                Currency_Symbol = dto.Currency_Symbol,
                Registration_Number = dto.Registration_Number,
                Reg_Doc_Url = dto.Reg_Doc_Url,
                Compliance_Status = dto.Compliance_Status,
                Website = dto.Website,
                Contact_Email = dto.Contact_Email,
                Contact_Phone = dto.Contact_Phone,
                Api_Endpoint = dto.Api_Endpoint,
                Sandbox_Endpoint = dto.Sandbox_Endpoint,
                Webhook_Url = dto.Webhook_Url,
                Api_Key = dto.Api_Key,
                Documentation_Url = dto.Documentation_Url,
                Integration_Type = dto.Integration_Type,
                Supported_Payment_Methods = dto.Supported_Payment_Methods,
                Supported_Currencies = dto.Supported_Currencies,
                Settlement_Frequency = dto.Settlement_Frequency,
                Transaction_Limit = dto.Transaction_Limit,
                Daily_Volume_Limit = dto.Daily_Volume_Limit,
                Risk_Score = dto.Risk_Score,
                Requires_Kyc = dto.Requires_Kyc,
                Onboarded_By = dto.Onboarded_By,
                Onboarded_At = dto.Onboarded_At,
                BusinessLocation_Id = dto.Business_Location_Id,
                Business_Id = dto.Business_Id,
                Is_Active = true,
                Deleted = false,
                Published = true,
                RecordStatus = Blocks.RecordStatus.Active,
                Create_Date = DateTime.UtcNow,
                Create_User = Guid.Parse(userId)
            };

            await _uow.Psps.AddAsync(psp);
            await _uow.SaveAsync();

            // 🔥 CACHE INVALIDATION
            await _cache.RemoveAsync(PspCacheKeys.All);

            return MapToDto(psp);
        }

        public async Task<PspDto> UpdateAsync(Guid id, PspUpdateDto dto, string userId)
        {
            var entity = await _uow.Psps.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Psp not found");

            entity.Name = dto.Name;
            entity.Short_Name = dto.Short_Name;
            entity.Code = dto.Code;
            entity.Psp_Category_Id = dto.Psp_Category_Id;
            entity.Country_Id = dto.Country_Id;
            entity.Currency_Code = dto.Currency_Code;
            entity.Currency_Symbol = dto.Currency_Symbol;
            entity.Registration_Number = dto.Registration_Number;
            entity.Reg_Doc_Url = dto.Reg_Doc_Url;
            entity.Compliance_Status = dto.Compliance_Status;
            entity.Website = dto.Website;
            entity.Contact_Email = dto.Contact_Email;
            entity.Contact_Phone = dto.Contact_Phone;
            entity.Api_Endpoint = dto.Api_Endpoint;
            entity.Sandbox_Endpoint = dto.Sandbox_Endpoint;
            entity.Webhook_Url = dto.Webhook_Url;
            entity.Api_Key = dto.Api_Key;
            entity.Documentation_Url = dto.Documentation_Url;
            entity.Integration_Type = dto.Integration_Type;
            entity.Supported_Payment_Methods = dto.Supported_Payment_Methods;
            entity.Supported_Currencies = dto.Supported_Currencies;
            entity.Settlement_Frequency = dto.Settlement_Frequency;
            entity.Transaction_Limit = dto.Transaction_Limit;
            entity.Daily_Volume_Limit = dto.Daily_Volume_Limit;
            entity.Risk_Score = dto.Risk_Score;
            entity.Requires_Kyc = dto.Requires_Kyc;
            entity.Onboarded_At = dto.Onboarded_At;
            entity.Onboarded_By = dto.Onboarded_By;

            entity.Last_Update_Date = DateTime.UtcNow;
            entity.Last_Update_User = Guid.Parse(userId);

            _uow.Psps.Update(entity);
            await _uow.SaveAsync();

            // 🔥 CACHE INVALIDATION
            await _cache.RemoveAsync(PspCacheKeys.All);
            await _cache.RemoveAsync(PspCacheKeys.ById(id));

            return MapToDto(entity);
        }

        public async Task<PspDto> DeleteAsync(Guid id, string userId)
        {
            var psp = await _uow.Psps.GetByIdAsync(id);
            if (psp == null) return new PspDto();

            psp.Deleted = true;
            psp.Published = false;
            psp.Is_Active = false;
            psp.RecordStatus = Blocks.RecordStatus.Inactive;
            psp.Last_Update_Date = DateTime.UtcNow;
            psp.Last_Update_User = Guid.Parse(userId);

            _uow.Psps.Update(psp);
            await _uow.SaveAsync();

            // 🔥 CACHE INVALIDATION
            await _cache.RemoveAsync(PspCacheKeys.All);
            await _cache.RemoveAsync(PspCacheKeys.ById(id));

            return MapToDto(psp);
        }

        private static PspDto MapToDto(Psp x) => new()
        {
            Id = x.Id,
            Name = x.Name,
            Short_Name = x.Short_Name,
            Code = x.Code,
            Psp_Category_Id = x.Psp_Category_Id,
            Psp_Category_Name = x.PspCategory?.Name ?? string.Empty,
            Country_Id = x.Country_Id,
            Country_Name = x.Country?.Name ?? string.Empty,
            Currency_Code = x.Currency_Code,
            Currency_Symbol = x.Currency_Symbol,
            Registration_Number = x.Registration_Number,
            Reg_Doc_Url = x.Reg_Doc_Url,
            Compliance_Status = x.Compliance_Status,
            Website = x.Website,
            Contact_Email = x.Contact_Email,
            Contact_Phone = x.Contact_Phone,
            Api_Endpoint = x.Api_Endpoint,
            Sandbox_Endpoint = x.Sandbox_Endpoint,
            Webhook_Url = x.Webhook_Url,
            Api_Key = x.Api_Key,
            Documentation_Url = x.Documentation_Url,
            Integration_Type = x.Integration_Type,
            Supported_Payment_Methods = x.Supported_Payment_Methods,
            Supported_Currencies = x.Supported_Currencies,
            Settlement_Frequency = x.Settlement_Frequency,
            Transaction_Limit = x.Transaction_Limit,
            Daily_Volume_Limit = x.Daily_Volume_Limit,
            Risk_Score = x.Risk_Score,
            Requires_Kyc = x.Requires_Kyc,
            Onboarded_By = x.Onboarded_By,
            Onboarded_At = x.Onboarded_At,
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
