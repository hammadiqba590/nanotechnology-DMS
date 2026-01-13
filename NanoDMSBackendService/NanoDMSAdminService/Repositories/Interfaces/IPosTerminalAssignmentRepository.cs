using NanoDMSAdminService.Models;
using System.Linq.Expressions;

namespace NanoDMSAdminService.Repositories.Interfaces
{
    public interface IPosTerminalAssignmentRepository : IRepository<PosTerminalAssignment>
    {
        Task<IEnumerable<PosTerminalAssignment>> GetAllAsync();
        Task<IEnumerable<PosTerminalAssignment>> GetAllByConditionAsync(
    Expression<Func<PosTerminalAssignment, bool>> predicate);
        IQueryable<PosTerminalAssignment> GetQueryable();
        Task<PosTerminalAssignment?> GetByIdAsync(Guid id);
    }
}
