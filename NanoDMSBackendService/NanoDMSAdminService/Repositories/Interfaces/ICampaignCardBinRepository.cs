using NanoDMSAdminService.Models;
using System.Linq.Expressions;

namespace NanoDMSAdminService.Repositories.Interfaces
{
    public interface ICampaignCardBinRepository : IRepository<CampaignCardBin>
    {
        Task<IEnumerable<CampaignCardBin>> GetAllAsync();
        Task<IEnumerable<CampaignCardBin>> GetAllByConditionAsync(
    Expression<Func<CampaignCardBin, bool>> predicate);
        Task<CampaignCardBin?> GetByIdAsync(Guid id);
        IQueryable<CampaignCardBin> GetQueryable();
    }
}
