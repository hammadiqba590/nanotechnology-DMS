using NanoDMSAdminService.Models;
using System.Linq.Expressions;

namespace NanoDMSAdminService.Repositories.Interfaces
{
    public interface IPosTerminalConfigurationRepository : IRepository<PosTerminalConfiguration>
    {
        Task<IEnumerable<PosTerminalConfiguration>> GetAllAsync();
        Task<IEnumerable<PosTerminalConfiguration>> GetAllByConditionAsync(
    Expression<Func<PosTerminalConfiguration, bool>> predicate);
        IQueryable<PosTerminalConfiguration> GetQueryable();
        Task<PosTerminalConfiguration?> GetByIdAsync(Guid id);
    }
}
