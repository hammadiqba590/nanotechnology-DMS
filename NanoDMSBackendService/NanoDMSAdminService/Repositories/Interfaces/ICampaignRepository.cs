using NanoDMSAdminService.Data;
using NanoDMSAdminService.Models;
using System.Linq.Expressions;

namespace NanoDMSAdminService.Repositories.Interfaces
{
    public interface ICampaignRepository : IRepository<Campaign>
    {
        Task<IEnumerable<Campaign>> GetAllAsync();
        Task<IEnumerable<Campaign>> GetAllByConditionAsync(
    Expression<Func<Campaign, bool>> predicate);
        Task<Campaign?> GetByIdAsync(Guid id);
        IQueryable<Campaign> GetQueryable();
        Task<Campaign?> GetWithBanksAsync(Guid id);
        
    }


}
