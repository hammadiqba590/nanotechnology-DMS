using NanoDMSAdminService.Models;
using System.Linq.Expressions;

namespace NanoDMSAdminService.Repositories.Interfaces
{
    public interface IDiscountRuleRepository : IRepository<DiscountRule>
    {
        Task<IEnumerable<DiscountRule>> GetAllAsync();
        Task<IEnumerable<DiscountRule>> GetAllByConditionAsync(
    Expression<Func<DiscountRule, bool>> predicate);
        IQueryable<DiscountRule> GetQueryable();
        Task<DiscountRule?> GetByIdAsync(Guid id);
        
    }

}
