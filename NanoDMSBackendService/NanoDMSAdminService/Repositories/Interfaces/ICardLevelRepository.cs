using NanoDMSAdminService.Models;
using System.Linq.Expressions;

namespace NanoDMSAdminService.Repositories.Interfaces
{
    public interface ICardLevelRepository : IRepository<CardLevel>
    {
        Task<IEnumerable<CardLevel>> GetAllAsync();
        Task<IEnumerable<CardLevel>> GetAllByConditionAsync(
    Expression<Func<CardLevel, bool>> predicate);
        Task<CardLevel?> GetByIdAsync(Guid id);
        IQueryable<CardLevel> GetQueryable();
    }
}
