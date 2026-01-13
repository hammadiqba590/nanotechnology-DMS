using Microsoft.EntityFrameworkCore;
using NanoDMSAdminService.Data;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Repositories.Interfaces;
using System.Linq;
using System.Linq.Expressions;

namespace NanoDMSAdminService.Repositories.Implementations
{
    public class CardTypeRepository : ICardTypeRepository
    {
        private readonly AppDbContext _context;
        public CardTypeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(CardType cardType)
        => await _context.CardTypes.AddAsync(cardType);

        public void Delete(CardType cardType)
        => _context.CardTypes.Remove(cardType);

        public async Task<IEnumerable<CardType>> GetAllAsync()
        => await _context.CardTypes
                .Where(x => !x.Deleted)
                .Include(x => x.Card_Bins)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<CardType>> GetAllByConditionAsync(Expression<Func<CardType, bool>> predicate)
        {
            return await _context.CardTypes
              .Where(predicate)
              .Include(x => x.Card_Bins)
              .AsNoTracking()
              .ToListAsync();
        }

        public async Task<CardType?> GetByIdAsync(Guid id)
        => await _context.CardTypes
              .Include(x => x.Card_Bins)
              .AsNoTracking()
              .FirstOrDefaultAsync(x => x.Id == id);

        public IQueryable<CardType> GetQueryable()
        => _context.CardTypes
              .Include(x => x.Card_Bins)
              .AsNoTracking()
              .AsQueryable();

        public void Update(CardType cardType)
        => _context.CardTypes.Update(cardType);
    }
}
