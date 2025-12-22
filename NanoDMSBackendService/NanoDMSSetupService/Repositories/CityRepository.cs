using NanoDMSSetupService.Data;
using NanoDMSSetupService.Models;

namespace NanoDMSSetupService.Repositories
{
    public class CityRepository : Repository<City>,ICityRepository
    {
        public CityRepository(AppDbContext context) : base(context) { }

        
    }
}
