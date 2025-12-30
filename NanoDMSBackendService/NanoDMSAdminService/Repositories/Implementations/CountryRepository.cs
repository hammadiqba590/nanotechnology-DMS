using Microsoft.EntityFrameworkCore;
using NanoDMSAdminService.Data;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Repositories.Interfaces;
using System.Linq.Expressions;

namespace NanoDMSAdminService.Repositories.Implementations
{
    public class CountryRepository : ICountryRepository
    {
        private readonly AppDbContext _context;

        public CountryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Country>> GetAllAsync()
            => await _context.Countries
                .Where(x => !x.Deleted)
                .Include(x=> x.Card_Bins)
                .Include(x => x.Currencies)
                .Include(x => x.Banks)
                .Include(x => x.Psps)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<Country>> GetAllByConditionAsync(
    Expression<Func<Country, bool>> predicate)
        {
            return await _context.Countries
                .Where(predicate)
                .Include(x => x.Card_Bins)
                .Include(x => x.Currencies)
                .Include(x => x.Banks)
                .Include(x => x.Psps)
                .ToListAsync();
        }

        public async Task<Country?> GetByIdAsync(Guid id)
            => await _context.Countries.Include(x => x.Card_Bins)
            .Include(x => x.Currencies)
            .Include(x => x.Banks)
            .Include(x => x.Psps).FirstOrDefaultAsync(x => x.Id == id);

        public IQueryable<Country> GetQueryable()
            => _context.Countries.Include(x => x.Card_Bins)
            .Include(x => x.Currencies)
            .Include(x => x.Banks)
            .Include(x => x.Psps).AsQueryable();

        public async Task AddAsync(Country country)
            => await _context.Countries.AddAsync(country);

        public void Update(Country country)
            => _context.Countries.Update(country);
    }

}
