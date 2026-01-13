using NanoDMSAdminService.Models;
using System.Linq.Expressions;

namespace NanoDMSAdminService.Repositories.Interfaces
{
    public interface ICardTypeRepository : IRepository<CardType>
    {
        Task<IEnumerable<CardType>> GetAllAsync();
        Task<IEnumerable<CardType>> GetAllByConditionAsync(
    Expression<Func<CardType, bool>> predicate);
        Task<CardType?> GetByIdAsync(Guid id);
        IQueryable<CardType> GetQueryable();
    }
}
