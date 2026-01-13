using NanoDMSAdminService.Models;
using System.Linq.Expressions;

namespace NanoDMSAdminService.Repositories.Interfaces
{
    public interface IPosTerminalMasterRepository : IRepository<PosTerminalMaster>
    {
        Task<IEnumerable<PosTerminalMaster>> GetAllAsync();
        Task<IEnumerable<PosTerminalMaster>> GetAllByConditionAsync(
    Expression<Func<PosTerminalMaster, bool>> predicate);
        IQueryable<PosTerminalMaster> GetQueryable();
        Task<PosTerminalMaster?> GetByIdAsync(Guid id);
    }
}
