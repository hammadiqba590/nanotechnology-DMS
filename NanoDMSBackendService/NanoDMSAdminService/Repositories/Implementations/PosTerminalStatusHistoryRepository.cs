using Microsoft.EntityFrameworkCore;
using NanoDMSAdminService.Data;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Repositories.Interfaces;
using System.Linq;
using System.Linq.Expressions;

namespace NanoDMSAdminService.Repositories.Implementations
{
    public class PosTerminalStatusHistoryRepository : IPosTerminalStatusHistoryRepository
    {
        private readonly AppDbContext _context;

        public PosTerminalStatusHistoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PosTerminalStatusHistory posTerminalStatusHistory)
        => await _context.PosTerminalStatusHistories.AddAsync(posTerminalStatusHistory);

        public void Delete(PosTerminalStatusHistory posTerminalStatusHistory)
        => _context.PosTerminalStatusHistories.Remove(posTerminalStatusHistory);

        public async Task<IEnumerable<PosTerminalStatusHistory>> GetAllAsync()
        => await _context.PosTerminalStatusHistories
                .Where(x => !x.Deleted)
                .Include(x => x.Pos_Terminal)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<PosTerminalStatusHistory>> GetAllByConditionAsync(Expression<Func<PosTerminalStatusHistory, bool>> predicate)
        {
            return await _context.PosTerminalStatusHistories
                 .Where(predicate)
                 .Include(x => x.Pos_Terminal)
                 .ToListAsync();
        }

        public async Task<PosTerminalStatusHistory?> GetByIdAsync(Guid id)
       => await _context.PosTerminalStatusHistories
                .Include(x => x.Pos_Terminal)
                .FirstOrDefaultAsync(x => x.Id == id);

        public IQueryable<PosTerminalStatusHistory> GetQueryable()
       => _context.PosTerminalStatusHistories
                .Include(x => x.Pos_Terminal)
                .AsQueryable();

        public void Update(PosTerminalStatusHistory posTerminalStatusHistory)
         => _context.PosTerminalStatusHistories.Update(posTerminalStatusHistory);
    }
}
