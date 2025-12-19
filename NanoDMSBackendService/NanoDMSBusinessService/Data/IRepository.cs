using System.Linq.Expressions;

namespace NanoDMSBusinessService.Data
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

        // New method to get data based on UserId or any custom condition
        Task<T> GetByConditionAsync(Expression<Func<T, bool>> predicate);

        // ✅ New method to get all records based on a condition
        Task<IEnumerable<T>> GetAllByConditionAsync(Expression<Func<T, bool>> predicate);
    }
}
