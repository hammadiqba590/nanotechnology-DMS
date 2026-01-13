using Microsoft.EntityFrameworkCore;
using NanoDMSAdminService.Data;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Repositories.Interfaces;
using System.Linq;
using System.Linq.Expressions;

namespace NanoDMSAdminService.Repositories.Implementations
{
    public class PspPaymentMethodRepository : IPspPaymentMethodRepository
    {
        private readonly AppDbContext _context;

        public PspPaymentMethodRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PspPaymentMethod pspPaymentMethod)
        => await _context.PspPaymentMethods.AddAsync(pspPaymentMethod);

        public void Delete(PspPaymentMethod pspPaymentMethod)
        => _context.PspPaymentMethods.Remove(pspPaymentMethod);

        public async Task<IEnumerable<PspPaymentMethod>> GetAllAsync()
       => await _context.PspPaymentMethods
                .Where(x => !x.Deleted)
                .Include(x => x.Psp)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<PspPaymentMethod>> GetAllByConditionAsync(Expression<Func<PspPaymentMethod, bool>> predicate)
        {
            return await _context.PspPaymentMethods
                      .Where(predicate)
                      .Include(x => x.Psp)
                      .ToListAsync();
        }

        public async Task<PspPaymentMethod?> GetByIdAsync(Guid id)
       => await _context.PspPaymentMethods
                .Include(x => x.Psp)
                .FirstOrDefaultAsync(x => x.Id == id);

        public IQueryable<PspPaymentMethod> GetQueryable()
        => _context.PspPaymentMethods
                .Include(x => x.Psp)
                .AsQueryable();

        public void Update(PspPaymentMethod pspPaymentMethod)
            => _context.PspPaymentMethods.Update(pspPaymentMethod);
    }
}
