using Microsoft.EntityFrameworkCore;
using NanoDMSAdminService.Data;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Repositories.Interfaces;
using System.Linq.Expressions;

namespace NanoDMSAdminService.Repositories.Implementations
{
    public class CampaignBankRepository : ICampaignBankRepository
    {
        private readonly AppDbContext _context;

        public CampaignBankRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(CampaignBank campaignBank)
         => await _context.CampaignBanks.AddAsync(campaignBank);

        public void Delete(CampaignBank campaignBank)
        => _context.CampaignBanks.Remove(campaignBank);

        public async Task<IEnumerable<CampaignBank>> GetAllAsync()
         => await _context.CampaignBanks
                .Where(x => !x.Deleted)
                .Include(x => x.Campaign_Card_Bins)
                .Include(x => x.Campaign)
                .Include(x => x.Bank)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<CampaignBank>> GetAllByConditionAsync(Expression<Func<CampaignBank, bool>> predicate)
        {
            return await _context.CampaignBanks
                .Where(predicate)
                .Include(x => x.Campaign_Card_Bins)
                .Include(x => x.Campaign)
                .Include(x => x.Bank)
                .ToListAsync();
        }

        public async Task<CampaignBank?> GetByIdAsync(Guid id)
        => await _context.CampaignBanks
                .Include(x => x.Campaign_Card_Bins)
                .Include(x => x.Campaign)
                .Include(x => x.Bank)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

        public IQueryable<CampaignBank> GetQueryable()
        => _context.CampaignBanks
            .Include(x => x.Campaign_Card_Bins)
            .Include(x => x.Campaign)
            .Include(x => x.Bank).AsQueryable();

        public void Update(CampaignBank campaignBank)
        => _context.CampaignBanks.Update(campaignBank);
    }
}
