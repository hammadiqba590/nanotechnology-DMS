using NanoDMSAdminService.Models;
using System.Linq.Expressions;

namespace NanoDMSAdminService.Repositories.Interfaces
{
    public interface ICurrencyRepository
    {
        Task<IEnumerable<Currency>> GetAllAsync();
        Task<IEnumerable<Currency>> GetAllByConditionAsync(
    Expression<Func<Currency, bool>> predicate);
        Task<Currency?> GetByIdAsync(Guid id);
        IQueryable<Currency> GetQueryable();
        Task AddAsync(Currency currency);
        void Update(Currency currency);
    }
}
