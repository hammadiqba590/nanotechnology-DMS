using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace NanoDMSBusinessService.Data
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();

        public async Task<T> GetByIdAsync(Guid id) =>
            await _context.Set<T>().FindAsync(id) ?? throw new InvalidOperationException($"Entity of type {typeof(T).Name} with ID {id} not found.");

        public async Task AddAsync(T entity) => await _context.Set<T>().AddAsync(entity);

        public async Task<T> GetByConditionAsync(Expression<Func<T, bool>> predicate) =>
            await _dbSet.FirstOrDefaultAsync(predicate) ?? throw new InvalidOperationException($"Entity of type {typeof(T).Name} matching the condition was not found.");

        public async Task<IEnumerable<T>> GetAllByConditionAsync(Expression<Func<T, bool>> predicate) =>
            await _dbSet.Where(predicate).ToListAsync();

        public void Update(T entity) => _context.Set<T>().Update(entity);

        public void Delete(T entity) => _context.Set<T>().Remove(entity);

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

        public async Task<IEnumerable<T>> GetPublishedAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }
    }
}
