using Microsoft.EntityFrameworkCore;
using NanoDMSAdminService.Data;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Repositories.Interfaces;
using System.Linq;
using System.Linq.Expressions;

namespace NanoDMSAdminService.Repositories.Implementations
{
    public class PspCategoryRepository : IPspCategoryRepository
    {
        private readonly AppDbContext _context;

        public PspCategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PspCategory pspCategory)
         => await _context.PspCategories.AddAsync(pspCategory);

        public void Delete(PspCategory pspCategory)
        => _context.PspCategories.Remove(pspCategory);

        public async Task<IEnumerable<PspCategory>> GetAllAsync()
        => await _context.PspCategories
                .Where(x => !x.Deleted)
                .Include(x => x.Psps)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<PspCategory>> GetAllByConditionAsync(Expression<Func<PspCategory, bool>> predicate)
        {
            return await _context.PspCategories
                 .Where(predicate)
                 .Include(x => x.Psps)
                 .ToListAsync();
        }

        public async Task<PspCategory?> GetByIdAsync(Guid id)
        => await _context.PspCategories
                .Include(x => x.Psps)
                .FirstOrDefaultAsync(x => x.Id == id);

        public IQueryable<PspCategory> GetQueryable()
         => _context.PspCategories
                .Include(x => x.Psps)
                .AsQueryable();
        public void Update(PspCategory psp)
        => _context.PspCategories.Update(psp);
    }
}
