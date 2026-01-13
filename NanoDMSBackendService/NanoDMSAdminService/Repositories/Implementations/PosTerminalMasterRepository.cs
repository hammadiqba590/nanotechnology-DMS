using Microsoft.EntityFrameworkCore;
using NanoDMSAdminService.Data;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Repositories.Interfaces;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace NanoDMSAdminService.Repositories.Implementations
{
    public class PosTerminalMasterRepository : IPosTerminalMasterRepository
    {
        private readonly AppDbContext _context;

        public PosTerminalMasterRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PosTerminalMaster posTerminalMaster)
        => await _context.PosTerminalMasters.AddAsync(posTerminalMaster);

        public void Delete(PosTerminalMaster posTerminalMaster)
        => _context.PosTerminalMasters.Remove(posTerminalMaster);

        public async Task<IEnumerable<PosTerminalMaster>> GetAllAsync()
         => await _context.PosTerminalMasters
                .Where(x => !x.Deleted)
                .Include(x => x.PosTerminal_Assignments)
                .Include(x => x.PosTerminal_Configurations)
                .Include(x => x.PosTerminal_Status_Histories)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<PosTerminalMaster>> GetAllByConditionAsync(Expression<Func<PosTerminalMaster, bool>> predicate)
        {
            return await _context.PosTerminalMasters
                 .Where(predicate)
                 .Include(x => x.PosTerminal_Assignments)
                 .Include(x => x.PosTerminal_Configurations)
                 .Include(x => x.PosTerminal_Status_Histories)
                 .ToListAsync();
        }

        public async  Task<PosTerminalMaster?> GetByIdAsync(Guid id)
        => await _context.PosTerminalMasters
                .Include(x => x.PosTerminal_Assignments)
                .Include(x => x.PosTerminal_Configurations)
                .Include(x => x.PosTerminal_Status_Histories)
                .FirstOrDefaultAsync(x => x.Id == id);

        public IQueryable<PosTerminalMaster> GetQueryable()
       => _context.PosTerminalMasters
                .Include(x => x.PosTerminal_Assignments)
                .Include(x => x.PosTerminal_Configurations)
                .Include(x => x.PosTerminal_Status_Histories)
                .AsQueryable();

        public void Update(PosTerminalMaster posTerminalMaster)
         => _context.PosTerminalMasters.Update(posTerminalMaster);
    }
}
