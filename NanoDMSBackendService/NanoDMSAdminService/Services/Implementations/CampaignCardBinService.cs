using Microsoft.EntityFrameworkCore;
using NanoDMSAdminService.Blocks;
using NanoDMSAdminService.Common;
using NanoDMSAdminService.DTO.CampaignBank;
using NanoDMSAdminService.DTO.CampaignCardBin;
using NanoDMSAdminService.Filters;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Services.Interfaces;
using NanoDMSAdminService.UnitOfWorks;

namespace NanoDMSAdminService.Services.Implementations
{
    public class CampaignCardBinService : ICampaignCardBinService
    {
        private readonly IUnitOfWork _uow;

        public CampaignCardBinService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CampaignCardBinDto> CreateAsync(CampaignCardBinCreateDto dto, string userId)
        {
            var entity = new CampaignCardBin
            {
                Id = Guid.NewGuid(),
                Campagin_Bank_Id = dto.Campagin_Bank_Id,
                Card_Bin_Id = dto.Card_Bin_Id,
                Status = dto.Status ?? CampaginCardBinStatus.Active,
                RecordStatus = RecordStatus.Active,

                Published = true,
                Deleted = false,
                Is_Active = true,
                Create_Date = DateTime.UtcNow,
                Create_User = Guid.Parse(userId),
                Business_Id = dto.Business_Id,
                BusinessLocation_Id = dto.Business_Location_Id
            };

            await _uow.CampaignCardBins.AddAsync(entity);
            await _uow.SaveAsync();

            return MapToDto(entity);
        }

        public async Task<CampaignCardBinDto> DeleteAsync(Guid id, string userId)
        {
            var campaignCardBin = await _uow.CampaignCardBins.GetByIdAsync(id);
            if (campaignCardBin == null) return new CampaignCardBinDto();

            campaignCardBin.Deleted = true;
            campaignCardBin.Published = false;
            campaignCardBin.Is_Active = false;
            campaignCardBin.Status = CampaginCardBinStatus.Inactive;
            campaignCardBin.RecordStatus = Blocks.RecordStatus.Inactive;
            campaignCardBin.Last_Update_Date = DateTime.UtcNow;
            campaignCardBin.Last_Update_User = Guid.Parse(userId);

            _uow.CampaignCardBins.Update(campaignCardBin);
            await _uow.SaveAsync();

            return MapToDto(campaignCardBin);
        }

        public async Task<IEnumerable<CampaignCardBinDto>> GetAllAsync()
        {
            var campaignCardBin = await _uow.CampaignCardBins.GetAllByConditionAsync(b =>
               !b.Deleted && b.Is_Active
           );

            return campaignCardBin.Select(x => new CampaignCardBinDto
            {
                Id = x.Id,
                Campagin_Bank_Id = x.Campagin_Bank_Id,
                Card_Bin_Id = x.Card_Bin_Id,
                Status = x.Status,
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

        public async Task<CampaignCardBinDto?> GetByIdAsync(Guid id)
        {
            var campaignCardBin = await _uow.CampaignCardBins.GetByIdAsync(id);
            return campaignCardBin == null ? null : MapToDto(campaignCardBin);
        }

        public async Task<PaginatedResponseDto<CampaignCardBinDto>> GetPagedAsync(CampaignCardBinFilterModel filter)
        {
            var query = _uow.CampaignCardBins.GetQueryable(); 

            if (filter.Status.HasValue)
                query = query.Where(x => x.Status == filter.Status);

            query = query.OrderByDescending(x => x.Create_Date);

            var totalRecords = await query.CountAsync();

            var campaigns = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PaginatedResponseDto<CampaignCardBinDto>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / filter.PageSize),
                Data = campaigns.Select(MapToDto).ToList()
            };
        }

        public async Task<CampaignCardBinDto> UpdateAsync(Guid id, CampaignCardBinUpdateDto dto, string userId)
        {
            var entity = await _uow.CampaignCardBins.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Campaign CardBin not found");

            entity.Campagin_Bank_Id = dto.Campagin_Bank_Id;
            entity.Card_Bin_Id = dto.Card_Bin_Id;
            entity.Status = dto.Status;

            entity.Last_Update_Date = DateTime.UtcNow;
            entity.Last_Update_User = Guid.Parse(userId);

            _uow.CampaignCardBins.Update(entity);
            await _uow.SaveAsync();
            return MapToDto(entity);
        }

        private static CampaignCardBinDto MapToDto(CampaignCardBin x) => new()
        {
            Id = x.Id,
            Campagin_Bank_Id = x.Campagin_Bank_Id,
            Card_Bin_Id = x.Card_Bin_Id,
            Status = x.Status,
            Business_Id = x.Business_Id,
            BusinessLocation_Id = x.BusinessLocation_Id,
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
