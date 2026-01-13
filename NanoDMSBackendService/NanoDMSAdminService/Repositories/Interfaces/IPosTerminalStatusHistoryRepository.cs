using NanoDMSAdminService.Models;
using System.Linq.Expressions;

namespace NanoDMSAdminService.Repositories.Interfaces
{
    public interface IPosTerminalStatusHistoryRepository : IRepository<PosTerminalStatusHistory>
    {
        Task<IEnumerable<PosTerminalStatusHistory>> GetAllAsync();
        Task<IEnumerable<PosTerminalStatusHistory>> GetAllByConditionAsync(
    Expression<Func<PosTerminalStatusHistory, bool>> predicate);
        IQueryable<PosTerminalStatusHistory> GetQueryable();
        Task<PosTerminalStatusHistory?> GetByIdAsync(Guid id);
    }
}
