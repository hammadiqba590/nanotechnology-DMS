using System.Linq.Expressions;

namespace NanoDMSSetupService.Data
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(Guid id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task SaveChangesAsync();

        // Add this for filtering by Published flag or similar conditions
        Task<IEnumerable<T>> GetPublishedAsync(Expression<Func<T, bool>> predicate);
    }

}
