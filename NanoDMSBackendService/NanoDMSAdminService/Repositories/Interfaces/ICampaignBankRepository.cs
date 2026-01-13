using NanoDMSAdminService.Models;
using System.Linq.Expressions;

namespace NanoDMSAdminService.Repositories.Interfaces
{
    public interface ICampaignBankRepository : IRepository<CampaignBank>
    {
        Task<IEnumerable<CampaignBank>> GetAllAsync();
        Task<IEnumerable<CampaignBank>> GetAllByConditionAsync(
    Expression<Func<CampaignBank, bool>> predicate);
        Task<CampaignBank?> GetByIdAsync(Guid id);
        IQueryable<CampaignBank> GetQueryable();
        
    }
}
