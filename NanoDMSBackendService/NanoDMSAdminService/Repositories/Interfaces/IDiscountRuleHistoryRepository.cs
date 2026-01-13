using NanoDMSAdminService.Models;
using System.Linq.Expressions;

namespace NanoDMSAdminService.Repositories.Interfaces
{
    public interface IDiscountRuleHistoryRepository : IRepository<DiscountRuleHistory>
    {
        Task<IEnumerable<DiscountRuleHistory>> GetAllAsync();
        Task<IEnumerable<DiscountRuleHistory>> GetAllByConditionAsync(
    Expression<Func<DiscountRuleHistory, bool>> predicate);
        IQueryable<DiscountRuleHistory> GetQueryable();
        Task<DiscountRuleHistory?> GetByIdAsync(Guid id);
        
    }
}
