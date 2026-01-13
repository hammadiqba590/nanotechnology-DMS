using NanoDMSAdminService.Models;
using System.Linq.Expressions;

namespace NanoDMSAdminService.Repositories.Interfaces
{
    public interface ICurrencyRepository : IRepository<Currency>
    {
        Task<IEnumerable<Currency>> GetAllAsync();
        Task<IEnumerable<Currency>> GetAllByConditionAsync(
    Expression<Func<Currency, bool>> predicate);
        Task<Currency?> GetByIdAsync(Guid id);
        IQueryable<Currency> GetQueryable();
       
    }
}
