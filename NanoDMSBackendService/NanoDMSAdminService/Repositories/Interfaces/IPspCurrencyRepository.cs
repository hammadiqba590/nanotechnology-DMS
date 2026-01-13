using NanoDMSAdminService.Models;
using System.Linq.Expressions;

namespace NanoDMSAdminService.Repositories.Interfaces
{
    public interface IPspCurrencyRepository : IRepository<PspCurrency>
    {
        Task<IEnumerable<PspCurrency>> GetAllAsync();
        Task<IEnumerable<PspCurrency>> GetAllByConditionAsync(
    Expression<Func<PspCurrency, bool>> predicate);
        IQueryable<PspCurrency> GetQueryable();
        Task<PspCurrency?> GetByIdAsync(Guid id);
    }
}
