using Microsoft.EntityFrameworkCore;
using NanoDMSAdminService.Data;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Repositories.Interfaces;
using System.Linq;
using System.Linq.Expressions;

namespace NanoDMSAdminService.Repositories.Implementations
{
    public class CardLevelRepository : ICardLevelRepository
    {
        private readonly AppDbContext _context;
        public CardLevelRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(CardLevel cardLevel)
        => await _context.CardLevels.AddAsync(cardLevel);

        public void Delete(CardLevel cardLevel)
        => _context.CardLevels.Remove(cardLevel);

        public async Task<IEnumerable<CardLevel>> GetAllAsync()
        => await _context.CardLevels
                .Where(x => !x.Deleted)
                .Include(x => x.Card_Bins)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<CardLevel>> GetAllByConditionAsync(Expression<Func<CardLevel, bool>> predicate)
        {
            return await _context.CardLevels
              .Where(predicate)
              .Include(x => x.Card_Bins)
              .AsNoTracking()
              .ToListAsync();
        }

        public async Task<CardLevel?> GetByIdAsync(Guid id)
        => await _context.CardLevels
              .Include(x => x.Card_Bins)
              .AsNoTracking()
              .FirstOrDefaultAsync(x => x.Id == id);

        public  IQueryable<CardLevel> GetQueryable()
        => _context.CardLevels
              .Include(x => x.Card_Bins)
              .AsNoTracking()
              .AsQueryable();

        public void Update(CardLevel cardLevel)
        => _context.CardLevels.Update(cardLevel);
    }
}
