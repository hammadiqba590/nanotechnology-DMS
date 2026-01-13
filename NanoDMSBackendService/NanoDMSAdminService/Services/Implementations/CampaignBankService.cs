using Microsoft.EntityFrameworkCore;
using NanoDMSAdminService.Blocks;
using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.Campagin;
using NanoDMSAdminService.DTO.CampaignBank;
using NanoDMSAdminService.Filters;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Services.Interfaces;
using NanoDMSAdminService.UnitOfWorks;

namespace NanoDMSAdminService.Services.Implementations
{
    public class CampaignBankService : ICampaignBankService
    {
        private readonly IUnitOfWork _uow;
        
        public CampaignBankService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CampaignBankDto> CreateAsync(CampaignBankCreateDto dto, string userId)
        {
            var entity = new CampaignBank
            {
                Id = Guid.NewGuid(),
                Campagin_Id = dto.Campagin_Id,
                Bank_Id = dto.Bank_Id,
                Budget = dto.Budget,
                Discount_Share = dto.Discount_Share,
                Tax_On_Merchant_Share = dto.Tax_On_Merchant_Share,
                Budget_Limit_Type = dto.Budget_Limit_Type,
                Budget_Limit_Value = dto.Budget_Limit_Value,
                Discount_Mode = dto.Discount_Mode,
                Status = RecordStatus.Active,
                RecordStatus = RecordStatus.Active,

                Published = true,
                Deleted = false,
                Is_Active = true,
                Create_Date = DateTime.UtcNow,
                Create_User = Guid.Parse(userId),
                Business_Id = dto.Business_Id,
                BusinessLocation_Id = dto.Business_Location_Id
            };

            await _uow.CampaignBanks.AddAsync(entity);
            await _uow.SaveAsync();

            return MapToDto(entity);
        }

        public async Task<CampaignBankDto> DeleteAsync(Guid id, string userId)
        {
            var campaignBank = await _uow.CampaignBanks.GetByIdAsync(id);
            if (campaignBank == null) return new CampaignBankDto();

            campaignBank.Deleted = true;
            campaignBank.Published = false;
            campaignBank.Is_Active = false;
            campaignBank.Status = RecordStatus.Inactive;
            campaignBank.RecordStatus = Blocks.RecordStatus.Inactive;
            campaignBank.Last_Update_Date = DateTime.UtcNow;
            campaignBank.Last_Update_User = Guid.Parse(userId);

            _uow.CampaignBanks.Update(campaignBank);
            await _uow.SaveAsync();

            return MapToDto(campaignBank);
        }

        public async Task<IEnumerable<CampaignBankDto>> GetAllAsync()
        {
            var campaigns = await _uow.CampaignBanks.GetAllByConditionAsync(b =>
               !b.Deleted && b.Is_Active
           );

            return campaigns.Select(x => new CampaignBankDto
            {
                Id = x.Id,
                Campagin_Id = x.Campagin_Id,
                Bank_Id = x.Bank_Id,
                Budget = x.Budget,
                Discount_Mode = x.Discount_Mode,
                Discount_Share = x.Discount_Share,
                Tax_On_Merchant_Share = x.Tax_On_Merchant_Share,
                Status = x.Status,
                Budget_Limit_Type = x.Budget_Limit_Type,
                Budget_Limit_Value = x.Budget_Limit_Value,
                Is_Active = x.Is_Active,
                Deleted = x.Deleted,
                Published = x.Published,
                BusinessLocation_Id = x.BusinessLocation_Id,
                Business_Id = x.Business_Id,
                Create_Date = x.Create_Date,
                Create_User = x.Create_User,
                Last_Update_Date = x.Last_Update_Date,
                Last_Update_User = x.Last_Update_User,
                RecordStatus = x.RecordStatus
            });
        }

        public async Task<CampaignBankDto?> GetByIdAsync(Guid id)
        {
            var campaign = await _uow.CampaignBanks.GetByIdAsync(id);
            return campaign == null ? null : MapToDto(campaign);
        }

        public async Task<PaginatedResponseDto<CampaignBankDto>> GetPagedAsync(CampaignBankFilterModel filter)
        {
            var query = _uow.CampaignBanks.GetQueryable();

            if (filter.Tax_On_Merchant_Share.HasValue)
                query = query.Where(x => x.Tax_On_Merchant_Share == filter.Tax_On_Merchant_Share);

            if (filter.Budget_Limit_Type.HasValue)
                query = query.Where(x => x.Budget_Limit_Type == filter.Budget_Limit_Type);

            if (filter.Discount_Mode.HasValue)
                query = query.Where(x => x.Discount_Mode == filter.Discount_Mode);

            if (filter.Status.HasValue)
                query = query.Where(x => x.Status == filter.Status);

            query = query.OrderByDescending(x => x.Create_Date);

            var totalRecords = await query.CountAsync();

            var campaigns = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PaginatedResponseDto<CampaignBankDto>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / filter.PageSize),
                Data = campaigns.Select(MapToDto).ToList()
            };
        }

        public async Task<CampaignBankDto> UpdateAsync(Guid id, CampaignBankUpdateDto dto, string userId)
        {
            var entity = await _uow.CampaignBanks.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Campaign Bank not found");

            entity.Bank_Id = dto.Bank_Id;
            entity.Campagin_Id = dto.Campagin_Id;
            entity.Budget = dto.Budget;
            entity.Discount_Share = dto.Discount_Share;
            entity.Tax_On_Merchant_Share = dto.Tax_On_Merchant_Share;
            entity.Budget_Limit_Type = dto.Budget_Limit_Type;
            entity.Budget_Limit_Value = dto.Budget_Limit_Value;
            entity.Discount_Mode = dto.Discount_Mode;
            entity.Status = dto.Status;

            entity.Last_Update_Date = DateTime.UtcNow;
            entity.Last_Update_User = Guid.Parse(userId);

            _uow.CampaignBanks.Update(entity);
            await _uow.SaveAsync();

            return MapToDto(entity);
        }

        private static CampaignBankDto MapToDto(CampaignBank x) => new()
        {
            Id = x.Id, 
            Campagin_Id = x.Campagin_Id,
            Bank_Id = x.Bank_Id,
            Budget = x.Budget,
            Discount_Mode = x.Discount_Mode,
            Discount_Share = x.Discount_Share,
            Tax_On_Merchant_Share = x.Tax_On_Merchant_Share,
            Status = x.Status,
            Business_Id = x.Business_Id,
            BusinessLocation_Id = x.BusinessLocation_Id,
            Budget_Limit_Type = x.Budget_Limit_Type,
            Budget_Limit_Value = x.Budget_Limit_Value,
            Is_Active = x.Is_Active,
            Deleted = x.Deleted,
            Published = x.Published,
            Create_Date = x.Create_Date,
            Create_User = x.Create_User,
            Last_Update_Date = x.Last_Update_Date,
            Last_Update_User = x.Last_Update_User,
            RecordStatus = x.RecordStatus
        };
    }
}
