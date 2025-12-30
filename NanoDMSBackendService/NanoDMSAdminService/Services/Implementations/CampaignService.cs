using NanoDMSAdminService.Models;
using NanoDMSAdminService.UnitOfWorks;

namespace NanoDMSAdminService.Services.Implementations
{
    public class CampaignService
    {
        private readonly IUnitOfWork _uow;

        public CampaignService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        //public async Task<long> CreateAsync(CreateCampaignDto dto, long userId)
        //{
        //    var campaign = new Campaign
        //    {
        //        Campaign_Name = dto.Name,
        //        Created_By = userId,
        //        Created_At = DateTime.UtcNow
        //    };

        //    await _uow.Campaigns.AddAsync(campaign);
        //    await _uow.SaveAsync();

        //    return campaign.Id;
        //}

        //public async Task<CampaignResponseDto?> GetAsync(long id)
        //{
        //    var entity = await _uow.Campaigns.GetWithBanksAsync(id);
        //    if (entity == null) return null;

        //    return CampaignMapper.ToDto(entity);
        //}
    }

}
