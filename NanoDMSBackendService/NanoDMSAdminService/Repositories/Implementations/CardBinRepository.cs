using Microsoft.EntityFrameworkCore;
using NanoDMSAdminService.Data;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Repositories.Interfaces;
using System.Linq;
using System.Linq.Expressions;

namespace NanoDMSAdminService.Repositories.Implementations
{
    public class CardBinRepository : ICardBinRepository
    {
        private readonly AppDbContext _context;

        public CardBinRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(CardBin cardBin)
        => await _context.CardBins.AddAsync(cardBin);

        public void Delete(CardBin cardBin)
        => _context.CardBins.Remove(cardBin);

        public async Task<IEnumerable<CardBin>> GetAllAsync()
        => await _context.CardBins
                .Where(x => !x.Deleted)
                .Include(x => x.Bank)
                .Include(x => x.Card_Brand)
                .Include(x => x.Card_Type)
                .Include(x => x.Card_Level)
                .Include(x => x.Country)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<CardBin>> GetAllByConditionAsync(Expression<Func<CardBin, bool>> predicate)
        {
            return await _context.CardBins
              .Where(predicate)
              .Include(x => x.Bank)
              .Include(x => x.Card_Brand)
              .Include(x => x.Card_Type)
              .Include(x => x.Card_Level)
              .Include(x => x.Country)
              .AsNoTracking()
              .ToListAsync();
        }

        public async Task<CardBin?> GetByIdAsync(Guid id)
         => await _context.CardBins
              .Include(x => x.Bank)
              .Include(x => x.Card_Brand)
              .Include(x => x.Card_Type)
              .Include(x => x.Card_Level)
              .Include(x => x.Country)
              .AsNoTracking()
              .FirstOrDefaultAsync(x => x.Id == id);

        public IQueryable<CardBin> GetQueryable()
        => _context.CardBins
              .Include(x => x.Bank)
              .Include(x => x.Card_Brand)
              .Include(x => x.Card_Type)
              .Include(x => x.Card_Level)
              .Include(x => x.Country)
              .AsNoTracking()
              .AsQueryable();

        public void Update(CardBin cardBin)
         => _context.CardBins.Update(cardBin);
    }
}
