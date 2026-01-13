using Microsoft.EntityFrameworkCore;
using NanoDMSAdminService.Blocks;
using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.Campagin;
using NanoDMSAdminService.DTO.Country;
using NanoDMSAdminService.Filters;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Services.Interfaces;
using NanoDMSAdminService.UnitOfWorks;
using System.Diagnostics.Metrics;

namespace NanoDMSAdminService.Services.Implementations
{
    public class CampaignService : ICampaignService
    {
        private readonly IUnitOfWork _uow;

        public CampaignService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CampaignDto> CreateAsync(CampaignCreateDto dto, string userId)
        {
            var campaign = new Campaign
            {
                Id = Guid.NewGuid(),
                Campaign_Name = dto.Campaign_Name,
                Description = dto.Description,
                Currency_Id = dto.Currency_Id,
                Tax_Amount = dto.Tax_Amount,
                Fbr = dto.Fbr,
                Budget_Limit_Type = dto.Budget_Limit_Type,
                Budget_Limit_Value = dto.Budget_Limit_Value,
                Priority = dto.Priority,
                Status = CampaginStatus.Active,

                Published = true,
                Deleted = false,
                Is_Active = true,
                Create_Date = DateTime.UtcNow,
                Create_User = Guid.Parse(userId),
                RecordStatus = Blocks.RecordStatus.Active,
                Business_Id = dto.Business_Id,
                BusinessLocation_Id = dto.Business_Location_Id
            };

            await _uow.Campaigns.AddAsync(campaign);
            await _uow.SaveAsync();

            return MapToDto(campaign);
        }

        public async Task<CampaignDto> DeleteAsync(Guid id, string userId)
        {
            var campaign = await _uow.Campaigns.GetByIdAsync(id);
            if (campaign == null) return new CampaignDto();

            campaign.Deleted = true;
            campaign.Published = false;
            campaign.Is_Active = false;
            campaign.Status = CampaginStatus.Inactive;
            campaign.RecordStatus = Blocks.RecordStatus.Inactive;
            campaign.Last_Update_Date = DateTime.UtcNow;
            campaign.Last_Update_User = Guid.Parse(userId);

            _uow.Campaigns.Update(campaign);
            await _uow.SaveAsync();

            return MapToDto(campaign);
        }

        public async Task<IEnumerable<CampaignDto>> GetAllAsync()
        {
            var campaigns = await _uow.Campaigns.GetAllByConditionAsync(b =>
               !b.Deleted && b.Is_Active
           );

            return campaigns.Select(b => new CampaignDto
            {
                Id = b.Id,
                Campaign_Name = b.Campaign_Name,
                Description = b.Description,
                Currency_Id = b.Currency_Id,
                Tax_Amount = b.Tax_Amount,
                Fbr = b.Fbr,
                Status = b.Status,
                Budget_Limit_Type = b.Budget_Limit_Type,
                Budget_Limit_Value = b.Budget_Limit_Value,
                Priority = b.Priority,
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

        public async Task<CampaignDto?> GetByIdAsync(Guid id)
        {
            var campaign = await _uow.Campaigns.GetByIdAsync(id);
            return campaign == null ? null : MapToDto(campaign);
        }

        public async Task<PaginatedResponseDto<CampaignDto>> GetPagedAsync(CampaignFilterModel filter)
        {
            var query = _uow.Campaigns.GetQueryable();

            if (!string.IsNullOrEmpty(filter.Campaign_Name))
                query = query.Where(x => x.Campaign_Name.Contains(filter.Campaign_Name));

            if (filter.Status.HasValue)
                query = query.Where(x => x.Status == filter.Status);

            query = query.OrderByDescending(x => x.Create_Date);

            var totalRecords = await query.CountAsync();

            var campaigns = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PaginatedResponseDto<CampaignDto>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / filter.PageSize),
                Data = campaigns.Select(MapToDto).ToList()
            };
        }

        public async Task<CampaignDto> UpdateAsync(Guid id, CampaignUpdateDto dto, string userId)
        {
            var campaign = await _uow.Campaigns.GetByIdAsync(id);
            if (campaign == null) return new CampaignDto();

            campaign.Campaign_Name = dto.Campaign_Name;
            campaign.Description = dto.Description;
            campaign.Currency_Id = dto.Currency_Id;
            campaign.Tax_Amount = dto.Tax_Amount;
            campaign.Fbr = dto.Fbr;
            campaign.Budget_Limit_Type = dto.Budget_Limit_Type;
            campaign.Budget_Limit_Value = dto.Budget_Limit_Value;
            campaign.Priority = dto.Priority;
            campaign.Status = dto.Status;

            campaign.Last_Update_Date = DateTime.UtcNow;
            campaign.Last_Update_User = Guid.Parse(userId);

            _uow.Campaigns.Update(campaign);
            await _uow.SaveAsync();

            return MapToDto(campaign);
        }

        private static CampaignDto MapToDto(Campaign x) => new()
        {
            Id = x.Id,
            Campaign_Name = x.Campaign_Name,
            Description = x.Description,
            Currency_Id = x.Currency_Id,
            Tax_Amount = x.Tax_Amount,
            Fbr = x.Fbr,
            Status = x.Status,
            Budget_Limit_Type = x.Budget_Limit_Type,
            Budget_Limit_Value = x.Budget_Limit_Value,
            Priority = x.Priority,
            Is_Active = x.Is_Active,
            Business_Id = x.Business_Id,
            BusinessLocation_Id = x.BusinessLocation_Id,
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
