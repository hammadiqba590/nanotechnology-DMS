using Microsoft.EntityFrameworkCore;
using NanoDMSAdminService.Data;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Repositories.Interfaces;
using System.Linq;
using System.Linq.Expressions;

namespace NanoDMSAdminService.Repositories.Implementations
{
    public class PspDocumentRepository : IPspDocumentRepository
    {
        private readonly AppDbContext _context;

        public PspDocumentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PspDocument pspDocument)
        => await _context.PspDocuments.AddAsync(pspDocument);

        public void Delete(PspDocument pspDocument)
        => _context.PspDocuments.Remove(pspDocument);

        public async Task<IEnumerable<PspDocument>> GetAllAsync()
        => await _context.PspDocuments
                .Where(x => !x.Deleted)
                .Include(x => x.Psp)
                .AsNoTracking()
                .ToListAsync();
        public async Task<IEnumerable<PspDocument>> GetAllByConditionAsync(Expression<Func<PspDocument, bool>> predicate)
        {
            return await _context.PspDocuments
                .Where(predicate)
                .Include(x => x.Psp)
                .ToListAsync();
        }

        public async Task<PspDocument?> GetByIdAsync(Guid id)
        => await _context.PspDocuments
                .Include(x => x.Psp)
                .FirstOrDefaultAsync(x => x.Id == id);

        public IQueryable<PspDocument> GetQueryable()
        => _context.PspDocuments
                .Include(x => x.Psp)
                .AsQueryable();

        public void Update(PspDocument pspDocument)
         => _context.PspDocuments.Update(pspDocument);
    }
}
