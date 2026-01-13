using NanoDMSAdminService.Models;
using System.Linq.Expressions;

namespace NanoDMSAdminService.Repositories.Interfaces
{
    public interface ICardBinRepository : IRepository<CardBin>
    {
        Task<IEnumerable<CardBin>> GetAllAsync();
        Task<IEnumerable<CardBin>> GetAllByConditionAsync(
    Expression<Func<CardBin, bool>> predicate);
        Task<CardBin?> GetByIdAsync(Guid id);
        IQueryable<CardBin> GetQueryable();
    }
}
