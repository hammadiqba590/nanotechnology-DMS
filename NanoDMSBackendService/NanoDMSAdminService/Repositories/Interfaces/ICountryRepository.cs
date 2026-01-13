using NanoDMSAdminService.Models;
using System.Linq.Expressions;

namespace NanoDMSAdminService.Repositories.Interfaces
{
    public interface ICountryRepository : IRepository<Country>
    {
        Task<IEnumerable<Country>> GetAllAsync();
        Task<IEnumerable<Country>> GetAllByConditionAsync(
    Expression<Func<Country, bool>> predicate);

        Task<Country?> GetByIdAsync(Guid id);
        IQueryable<Country> GetQueryable();
        
    }

}
