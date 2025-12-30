using NanoDMSAdminService.Data;
using NanoDMSAdminService.Models;

namespace NanoDMSAdminService.Repositories.Interfaces
{
    public interface ICampaignRepository
    {
        Task<Campaign?> GetByIdAsync(Guid id);
        Task<Campaign?> GetWithBanksAsync(Guid id);
        Task AddAsync(Campaign campaign);
        void Update(Campaign campaign);
        void Delete(Campaign campaign);
    }


}
