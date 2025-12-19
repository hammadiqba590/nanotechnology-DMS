using Microsoft.EntityFrameworkCore;
using NanoDMSBusinessService.Data;
using NanoDMSBusinessService.Models;

namespace NanoDMSBusinessService.Repositories
{
    public class BusinessLocationRepository : Repository<BusinessLocation>, IBusinessLocationRepository
    {
        public BusinessLocationRepository(AppDbContext context) : base(context) { }

        public async Task<int> GetLocationCountByBusinessIdAsync(Guid businessId)
        {
            return await _context.Set<BusinessLocation>()
                                 .Where(bl => bl.BusinessId == businessId).CountAsync();
                                 
        }
        public async Task<BusinessLocation?> GetByIdWithBusinessAsync(Guid id)
        {
            return await _context.BusinessLocation
                                   .Include(bl => bl.Business)      // 👈 grabs the parent row
                                   .FirstOrDefaultAsync(bl => bl.Id == id);
        }

        public async Task<List<BusinessLocation>> GetAllWithBusinessAsync()
        {
            return await _context.BusinessLocation
                                   .Include(bl => bl.Business)
                                   .ToListAsync();
        }


    }
}
