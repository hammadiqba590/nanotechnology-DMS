using NanoDMSAdminService.Data;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Repositories.Interfaces;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace NanoDMSAdminService.Repositories.Implementations
{
    public class DiscountRuleHistoryRepository : IDiscountRuleHistoryRepository
    {
        private readonly AppDbContext _context;

        public DiscountRuleHistoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DiscountRuleHistory>> GetAllAsync()
            => await _context.DiscountRuleHistories
                .Where(x => !x.Deleted)
                .Include(x => x.Currency)
                .Include(x => x.DiscountRule)
                .Include(x => x.CampaignCardBin)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<DiscountRuleHistory>> GetAllByConditionAsync(
    Expression<Func<DiscountRuleHistory, bool>> predicate)
        {
            return await _context.DiscountRuleHistories
                .Where(predicate)
                .Include(x => x.Currency)
                .Include(x => x.DiscountRule)
                .Include(x => x.CampaignCardBin)
                .ToListAsync();
        }

        public IQueryable<DiscountRuleHistory> GetQueryable()
            => _context.DiscountRuleHistories
                .Include(x => x.Currency)
                .Include(x => x.DiscountRule)
                .Include(x => x.CampaignCardBin)
                .AsQueryable();

        public async Task<DiscountRuleHistory?> GetByIdAsync(Guid id)
            => await _context.DiscountRuleHistories
                .Include(x => x.Currency)
                .Include(x => x.DiscountRule)
                .Include(x => x.CampaignCardBin)
                .FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddAsync(DiscountRuleHistory rule)
            => await _context.DiscountRuleHistories.AddAsync(rule);

        public void Update(DiscountRuleHistory rule)
            => _context.DiscountRuleHistories.Update(rule);
    }
}
