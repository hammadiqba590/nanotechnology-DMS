using Microsoft.EntityFrameworkCore;
using NanoDMSAdminService.Data;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Repositories.Interfaces;
using System.Linq.Expressions;

namespace NanoDMSAdminService.Repositories.Implementations
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly AppDbContext _context;

        public CurrencyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Currency>> GetAllAsync()
            => await _context.Currencies
                .Where(x => !x.Deleted)
                .Include(x => x.Country)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<Currency>> GetAllByConditionAsync(
    Expression<Func<Currency, bool>> predicate)
        {
            return await _context.Currencies
                .Where(predicate)
                .Include(x => x.Country)
                .ToListAsync();
        }


        public async Task<Currency?> GetByIdAsync(Guid id)
            => await _context.Currencies
                .Include(x => x.Country)
                .FirstOrDefaultAsync(x => x.Id == id);

        public IQueryable<Currency> GetQueryable()
            => _context.Currencies
                .Include(x => x.Country)
                .AsQueryable();

        public async Task AddAsync(Currency currency)
            => await _context.Currencies.AddAsync(currency);

        public void Update(Currency currency)
            => _context.Currencies.Update(currency);
    }

}
