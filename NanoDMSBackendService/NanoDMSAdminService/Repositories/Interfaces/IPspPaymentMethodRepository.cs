using NanoDMSAdminService.Models;
using System.Linq.Expressions;

namespace NanoDMSAdminService.Repositories.Interfaces
{
    public interface IPspPaymentMethodRepository : IRepository<PspPaymentMethod>
    {
        Task<IEnumerable<PspPaymentMethod>> GetAllAsync();
        Task<IEnumerable<PspPaymentMethod>> GetAllByConditionAsync(
    Expression<Func<PspPaymentMethod, bool>> predicate);
        IQueryable<PspPaymentMethod> GetQueryable();
        Task<PspPaymentMethod?> GetByIdAsync(Guid id);
    }
}
