using Microsoft.EntityFrameworkCore;
using NanoDMSAdminService.Data;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Repositories.Interfaces;
using System.Linq;
using System.Linq.Expressions;

namespace NanoDMSAdminService.Repositories.Implementations
{
    public class PosTerminalConfigurationRepository : IPosTerminalConfigurationRepository
    {
        private readonly AppDbContext _context;

        public PosTerminalConfigurationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PosTerminalConfiguration posTerminalConfiguration)
        => await _context.PosTerminalConfigurations.AddAsync(posTerminalConfiguration);

        public void Delete(PosTerminalConfiguration posTerminalConfiguration)
        => _context.PosTerminalConfigurations.Remove(posTerminalConfiguration);

        public async  Task<IEnumerable<PosTerminalConfiguration>> GetAllAsync()
        => await _context.PosTerminalConfigurations
                .Where(x => !x.Deleted)
                .Include(x => x.Pos_Terminal)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<PosTerminalConfiguration>> GetAllByConditionAsync(Expression<Func<PosTerminalConfiguration, bool>> predicate)
        {
            return await _context.PosTerminalConfigurations
                 .Where(predicate)
                 .Include(x => x.Pos_Terminal)
                 .ToListAsync();
        }

        public async Task<PosTerminalConfiguration?> GetByIdAsync(Guid id)
        => await _context.PosTerminalConfigurations
                .Include(x => x.Pos_Terminal)
                .FirstOrDefaultAsync(x => x.Id == id);

        public IQueryable<PosTerminalConfiguration> GetQueryable()
        => _context.PosTerminalConfigurations
                .Include(x => x.Pos_Terminal)
                .AsQueryable();

        public void Update(PosTerminalConfiguration posTerminalConfiguration)
        => _context.PosTerminalConfigurations.Update(posTerminalConfiguration);
    }
}
