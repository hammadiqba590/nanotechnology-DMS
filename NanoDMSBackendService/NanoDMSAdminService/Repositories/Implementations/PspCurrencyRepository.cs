using Microsoft.EntityFrameworkCore;
using NanoDMSAdminService.Data;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Repositories.Interfaces;
using System.Linq;
using System.Linq.Expressions;

namespace NanoDMSAdminService.Repositories.Implementations
{
    public class PspCurrencyRepository : IPspCurrencyRepository
    {
        private readonly AppDbContext _context;

        public PspCurrencyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PspCurrency pspCurrency)
        => await _context.PspCurrencies.AddAsync(pspCurrency);

        public void Delete(PspCurrency pspCurrency)
        => _context.PspCurrencies.Remove(pspCurrency);

        public async Task<IEnumerable<PspCurrency>> GetAllAsync()
        => await _context.PspCurrencies
                .Where(x => !x.Deleted)
                .Include(x => x.Psp)
                .Include(x => x.Currency)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<PspCurrency>> GetAllByConditionAsync(Expression<Func<PspCurrency, bool>> predicate)
        {
            return await _context.PspCurrencies
                .Where(predicate)
                .Include(x => x.Psp)
                .Include(x => x.Currency)
                .ToListAsync();
        }

        public async Task<PspCurrency?> GetByIdAsync(Guid id)
         => await _context.PspCurrencies
                .Include(x => x.Psp)
                .Include(x => x.Currency)
                .FirstOrDefaultAsync(x => x.Id == id);


        public IQueryable<PspCurrency> GetQueryable()
        => _context.PspCurrencies
                .Include(x => x.Psp)
                .Include(x => x.Currency)
                .AsQueryable();

        public void Update(PspCurrency pspCurrency)
        => _context.PspCurrencies.Update(pspCurrency);
    }
}
