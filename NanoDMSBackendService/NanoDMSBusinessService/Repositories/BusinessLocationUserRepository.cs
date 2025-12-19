using Microsoft.EntityFrameworkCore;
using NanoDMSBusinessService.Data;
using NanoDMSBusinessService.Models;

namespace NanoDMSBusinessService.Repositories
{
    public class BusinessLocationUserRepository : Repository<BusinessLocationUser>, IBusinessLocationUserRepository
    {
        // Use the 'new' keyword to explicitly hide the inherited member
        private readonly new AppDbContext _context;

        public BusinessLocationUserRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddRangeAsync(List<BusinessLocationUser> businessLocationUsers)
        {
            if (businessLocationUsers == null || !businessLocationUsers.Any())
                throw new ArgumentException("The list of BusinessLocationUsers cannot be null or empty.");

            await _context.BusinessLocationUser.AddRangeAsync(businessLocationUsers);
        }

        public async Task<IEnumerable<BusinessLocationUser>> GetByUserIdAsync(Guid userId)
        {
            return await _context.BusinessLocationUser
                .Where(b => b.UserId == userId)
                .ToListAsync();
        }

        // Explicitly hide the inherited member using the 'new' keyword
        public new void Delete(BusinessLocationUser entity)
        {
            _context.BusinessLocationUser.Remove(entity);
        }

        // Explicitly hide the inherited member using the 'new' keyword
        public new async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }


}
