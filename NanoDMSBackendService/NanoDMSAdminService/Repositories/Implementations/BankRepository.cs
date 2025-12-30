using Microsoft.EntityFrameworkCore;
using NanoDMSAdminService.Data;
using NanoDMSAdminService.Filters;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Repositories.Interfaces;
using System.Linq.Expressions;

namespace NanoDMSAdminService.Repositories.Implementations
{
    // Repositories/BankRepository.cs
    public class BankRepository : IBankRepository
    {
        private readonly AppDbContext _context;

        public BankRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Bank>> GetAllAsync()
            => await _context.Banks
                .Include(b => b.Country)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<Bank>> GetAllByConditionAsync(
    Expression<Func<Bank, bool>> predicate)
        {
            return await _context.Banks
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Bank> Data, int TotalCount)> GetPagedAsync(BankFilterModel filter)
        {
            var query = _context.Banks
                .AsNoTracking()
                .Include(x => x.Country)
                .Where(x => !x.Deleted)
                .AsQueryable();

            // 🔎 Filters
            if (!string.IsNullOrWhiteSpace(filter.Name))
                query = query.Where(x => x.Name.Contains(filter.Name));

            if (filter.Country_Id.HasValue)
                query = query.Where(x => x.Country_Id == filter.Country_Id);

            if (filter.Is_Active.HasValue)
                query = query.Where(x => x.Is_Active == filter.Is_Active);

            // 🔢 Total Count
            var totalRecords = await query.CountAsync();

            // 📄 Pagination
            var data = await query
                .OrderByDescending(x => x.Create_Date)
                .Skip(filter.Skip)
                .Take(filter.PageSize)
                .ToListAsync();

            return (data, totalRecords);
        }

        public async Task<Bank?> GetByIdAsync(Guid id)
            => await _context.Banks
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);

        public async Task<Bank?> GetWithCountryAsync(Guid id)
            => await _context.Banks
                .Include(b => b.Country)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);

        public async Task AddAsync(Bank bank)
            => await _context.Banks.AddAsync(bank);

        public void Update(Bank bank)
            => _context.Banks.Update(bank);

        public void Delete(Bank bank)
            => _context.Banks.Remove(bank);

        
    }

}
