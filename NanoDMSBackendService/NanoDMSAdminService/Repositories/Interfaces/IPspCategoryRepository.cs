using NanoDMSAdminService.Models;
using System.Linq.Expressions;

namespace NanoDMSAdminService.Repositories.Interfaces
{
    public interface IPspCategoryRepository : IRepository<PspCategory>
    {
        Task<IEnumerable<PspCategory>> GetAllAsync();
        Task<IEnumerable<PspCategory>> GetAllByConditionAsync(
    Expression<Func<PspCategory, bool>> predicate);
        IQueryable<PspCategory> GetQueryable();
        Task<PspCategory?> GetByIdAsync(Guid id);
    }
}
