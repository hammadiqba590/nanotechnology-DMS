using Microsoft.EntityFrameworkCore;
using NanoDMSAdminService.Data;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Repositories.Interfaces;
using System.Linq;
using System.Linq.Expressions;

namespace NanoDMSAdminService.Repositories.Implementations
{
    public class PspRepository : IPspRepository
    {
        private readonly AppDbContext _context;

        public PspRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Psp psp)
        => await _context.Psps.AddAsync(psp);

        public void Delete(Psp psp)
        => _context.Psps.Remove(psp);

        public async Task<IEnumerable<Psp>> GetAllAsync()
         => await _context.Psps
                .Where(x => !x.Deleted)
                .Include(x => x.PspCategory)
                .Include(x => x.Country)
                .Include(x => x.PspDocuments)
                .Include(x => x.PspPaymentMethods)
                .Include(x => x.PspCurrencies)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<Psp>> GetAllByConditionAsync(Expression<Func<Psp, bool>> predicate)
        {
            return await _context.Psps
                 .Where(predicate)
                 .Include(x => x.PspCategory)
                 .Include(x => x.Country)
                 .Include(x => x.PspDocuments)
                 .Include(x => x.PspPaymentMethods)
                 .Include(x => x.PspCurrencies)
                 .ToListAsync();
        }

        public async Task<Psp?> GetByIdAsync(Guid id)
            => await _context.Psps
                .Include(x => x.PspCategory)
                .Include(x => x.Country)
                .Include(x => x.PspDocuments)
                .Include(x => x.PspPaymentMethods)
                .Include(x => x.PspCurrencies)
                .FirstOrDefaultAsync(x => x.Id == id);

        public IQueryable<Psp> GetQueryable()
        => _context.Psps
                .Include(x => x.PspCategory)
                .Include(x => x.Country)
                .Include(x => x.PspDocuments)
                .Include(x => x.PspPaymentMethods)
                .Include(x => x.PspCurrencies)
                .AsQueryable();

        public void Update(Psp psp)
        => _context.Psps.Update(psp);
    }
}
