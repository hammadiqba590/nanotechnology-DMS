using Microsoft.EntityFrameworkCore;
using NanoDMSAdminService.Data;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Repositories.Interfaces;
using System.Linq;
using System.Linq.Expressions;

namespace NanoDMSAdminService.Repositories.Implementations
{
    public class CampaignCardBinRepository : ICampaignCardBinRepository
    {
        private readonly AppDbContext _context;

        public CampaignCardBinRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(CampaignCardBin campaignCardBin)
        => await _context.CampaignCardBins.AddAsync(campaignCardBin);

        public void Delete(CampaignCardBin campaignCardBin)
        => _context.CampaignCardBins.Remove(campaignCardBin);

        public async Task<IEnumerable<CampaignCardBin>> GetAllAsync()
         => await _context.CampaignCardBins
                .Where(x => !x.Deleted)
                .Include(x => x.Card_Bin)
                .Include(x => x.Campaign_Bank)
                .AsNoTracking()
                .ToListAsync();

        public async  Task<IEnumerable<CampaignCardBin>> GetAllByConditionAsync(Expression<Func<CampaignCardBin, bool>> predicate)
        {
            return await _context.CampaignCardBins
               .Where(predicate)
               .Include(x => x.Card_Bin)
               .Include(x => x.Campaign_Bank)
               .AsNoTracking()
               .ToListAsync();
        }

        public async Task<CampaignCardBin?> GetByIdAsync(Guid id)
        => await _context.CampaignCardBins
                .Include(x => x.Card_Bin)
                .Include(x => x.Campaign_Bank)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

        public IQueryable<CampaignCardBin> GetQueryable()
        => _context.CampaignCardBins
                .Include(x => x.Card_Bin)
                .Include(x => x.Campaign_Bank).AsNoTracking().AsQueryable();

        public void Update(CampaignCardBin campaignCardBin)
        => _context.CampaignCardBins.Update(campaignCardBin);
    }
}
