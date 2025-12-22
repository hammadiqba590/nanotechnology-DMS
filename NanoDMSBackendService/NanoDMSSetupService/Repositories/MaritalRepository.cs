using NanoDMSSetupService.Data;
using NanoDMSSetupService.Models;

namespace NanoDMSSetupService.Repositories
{
    public class MaritalRepository : Repository<MaritalStatus>, IMaritalRepository
    {
        public MaritalRepository(AppDbContext context) : base(context) { }

       
    }
}
