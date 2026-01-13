using Microsoft.EntityFrameworkCore;
using NanoDMSAdminService.Data;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Repositories.Interfaces;
using System.Linq;
using System.Linq.Expressions;

namespace NanoDMSAdminService.Repositories.Implementations
{
    public class PosTerminalAssignmentRepository : IPosTerminalAssignmentRepository
    {
        private readonly AppDbContext _context;
        public PosTerminalAssignmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PosTerminalAssignment posTerminalAssignment)
        => await _context.PosTerminalAssignments.AddAsync(posTerminalAssignment);

        public void Delete(PosTerminalAssignment posTerminalAssignment)
       => _context.PosTerminalAssignments.Remove(posTerminalAssignment);

        public async Task<IEnumerable<PosTerminalAssignment>> GetAllAsync()
        => await _context.PosTerminalAssignments
                .Where(x => !x.Deleted)
                .Include(x => x.Pos_Terminal)
                .AsNoTracking()
                .ToListAsync();

        public async Task<IEnumerable<PosTerminalAssignment>> GetAllByConditionAsync(Expression<Func<PosTerminalAssignment, bool>> predicate)
        {
            return await _context.PosTerminalAssignments
                 .Where(predicate)
                 .Include(x => x.Pos_Terminal)
                 .ToListAsync();
        }

        public async Task<PosTerminalAssignment?> GetByIdAsync(Guid id)
        => await _context.PosTerminalAssignments
                .Include(x => x.Pos_Terminal)
                .FirstOrDefaultAsync(x => x.Id == id);

        public IQueryable<PosTerminalAssignment> GetQueryable()
        => _context.PosTerminalAssignments
                .Include(x => x.Pos_Terminal)
                .AsQueryable();

        public void Update(PosTerminalAssignment posTerminalAssignment)
        => _context.PosTerminalAssignments.Update(posTerminalAssignment);
    }
}
