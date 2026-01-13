using NanoDMSAdminService.Models;
using System.Linq.Expressions;

namespace NanoDMSAdminService.Repositories.Interfaces
{
    public interface IPspRepository : IRepository<Psp>
    {
        Task<IEnumerable<Psp>> GetAllAsync();
        Task<IEnumerable<Psp>> GetAllByConditionAsync(
    Expression<Func<Psp, bool>> predicate);
        IQueryable<Psp> GetQueryable();
        Task<Psp?> GetByIdAsync(Guid id);
    }
}
