using NanoDMSAdminService.Models;
using System.Linq.Expressions;

namespace NanoDMSAdminService.Repositories.Interfaces
{
    public interface ICountryRepository
    {
        Task<IEnumerable<Country>> GetAllAsync();
        Task<IEnumerable<Country>> GetAllByConditionAsync(
    Expression<Func<Country, bool>> predicate);

        Task<Country?> GetByIdAsync(Guid id);
        IQueryable<Country> GetQueryable();
        Task AddAsync(Country country);
        void Update(Country country);
    }

}
