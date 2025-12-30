using System.Linq.Expressions;

namespace NanoDMSAdminService.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> Query(bool tracking = false);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
    }

}
