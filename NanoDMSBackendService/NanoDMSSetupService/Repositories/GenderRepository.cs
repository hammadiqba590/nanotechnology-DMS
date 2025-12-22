using NanoDMSSetupService.Data;
using NanoDMSSetupService.Models;

namespace NanoDMSSetupService.Repositories
{
    public class GenderRepository : Repository<Gender>,IGenderRepository
    {
        public GenderRepository(AppDbContext context) : base(context) { }

        
    }
}
