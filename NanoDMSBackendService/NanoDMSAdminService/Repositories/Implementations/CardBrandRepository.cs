using Microsoft.EntityFrameworkCore;
using NanoDMSAdminService.Data;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Repositories.Interfaces;
using System.Linq.Expressions;

namespace NanoDMSAdminService.Repositories.Implementations
{
    public class CardBrandRepository : ICardBrandRepository
    {
        private readonly AppDbContext _context;
        public CardBrandRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(CardBrand cardBrand)
       => await _context.CardBrands.AddAsync(cardBrand);

        public void Delete(CardBrand cardBrand)
        => _context.CardBrands.Remove(cardBrand);

        public async Task<IEnumerable<CardBrand>> GetAllAsync()
        => await _context.CardBrands
                .Where(x => !x.Deleted)
                .Include(x => x.Card_Bins)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<CardBrand>> GetAllByConditionAsync(Expression<Func<CardBrand, bool>> predicate)
        {
            return await _context.CardBrands
              .Where(predicate)
              .Include(x => x.Card_Bins)
              .AsNoTracking()
              .ToListAsync();
        }

        public async Task<CardBrand?> GetByIdAsync(Guid id)
         => await _context.CardBrands
              .Include(x => x.Card_Bins)
              .AsNoTracking()
              .FirstOrDefaultAsync(x => x.Id == id);

        public IQueryable<CardBrand> GetQueryable()
        => _context.CardBrands
              .Include(x => x.Card_Bins)
              .AsNoTracking()
              .AsQueryable();

        public void Update(CardBrand cardBrand)
         => _context.CardBrands.Update(cardBrand);
    }
}
