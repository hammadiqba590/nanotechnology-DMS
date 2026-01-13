using NanoDMSAdminService.Models;
using System.Linq.Expressions;

namespace NanoDMSAdminService.Repositories.Interfaces
{
    public interface IPspDocumentRepository : IRepository<PspDocument>
    {
        Task<IEnumerable<PspDocument>> GetAllAsync();
        Task<IEnumerable<PspDocument>> GetAllByConditionAsync(
    Expression<Func<PspDocument, bool>> predicate);
        IQueryable<PspDocument> GetQueryable();
        Task<PspDocument?> GetByIdAsync(Guid id);
    }
}
