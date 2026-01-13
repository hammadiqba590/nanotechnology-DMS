using NanoDMSAdminService.Models;
using System.Linq.Expressions;

namespace NanoDMSAdminService.Repositories.Interfaces
{
    public interface ICardBrandRepository : IRepository<CardBrand>
    {
        Task<IEnumerable<CardBrand>> GetAllAsync();
        Task<IEnumerable<CardBrand>> GetAllByConditionAsync(
    Expression<Func<CardBrand, bool>> predicate);
        Task<CardBrand?> GetByIdAsync(Guid id);
        IQueryable<CardBrand> GetQueryable();
    }
}
