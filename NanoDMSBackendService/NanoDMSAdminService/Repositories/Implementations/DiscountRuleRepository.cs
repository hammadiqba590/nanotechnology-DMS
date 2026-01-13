using Microsoft.EntityFrameworkCore;
using NanoDMSAdminService.Data;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Repositories.Interfaces;
using System.Data;
using System.Linq.Expressions;

namespace NanoDMSAdminService.Repositories.Implementations
{
    public class DiscountRuleRepository : IDiscountRuleRepository
    {
        private readonly AppDbContext _context;

        public DiscountRuleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DiscountRule>> GetAllAsync()
            => await _context.DiscountRules
                .Where(x => !x.Deleted)
                .Include(x => x.CampaignCardBin)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<DiscountRule>> GetAllByConditionAsync(
    Expression<Func<DiscountRule, bool>> predicate)
        {
            return await _context.DiscountRules
                .Where(predicate)
                .Include(x => x.CampaignCardBin)
                .ToListAsync();
        }

        public IQueryable<DiscountRule> GetQueryable()
            => _context.DiscountRules
                .Include(x => x.CampaignCardBin)
                .AsQueryable();

        public async Task<DiscountRule?> GetByIdAsync(Guid id)
            => await _context.DiscountRules
                .Include(x => x.CampaignCardBin)
                .FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddAsync(DiscountRule rule)
            => await _context.DiscountRules.AddAsync(rule);

        public void Update(DiscountRule rule)
            => _context.DiscountRules.Update(rule);

        public void Delete(DiscountRule rule)
        => _context.DiscountRules.Remove(rule);
    }

}
