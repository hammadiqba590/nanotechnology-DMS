using Microsoft.EntityFrameworkCore;
using NanoDMSAdminService.Data;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Repositories.Interfaces;
using System.Collections.Generic;

namespace NanoDMSAdminService.Repositories.Implementations
{
    public class CampaignRepository : ICampaignRepository
    {
        private readonly AppDbContext _context;

        public CampaignRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Campaign?> GetByIdAsync(Guid id)
            => await _context.Campaigns
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

        public async Task<Campaign?> GetWithBanksAsync(Guid id)
            => await _context.Campaigns
                .Include(x => x.CampaignBanks)
                    .ThenInclude(cb => cb.Bank)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddAsync(Campaign campaign)
            => await _context.Campaigns.AddAsync(campaign);

        public void Update(Campaign campaign)
            => _context.Campaigns.Update(campaign);

        public void Delete(Campaign campaign)
            => _context.Campaigns.Remove(campaign);
    }


}
