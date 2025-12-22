using NanoDMSSetupService.Data;
using NanoDMSSetupService.Models;

namespace NanoDMSSetupService.Repositories
{
    public class TimeZoneRepository: Repository<Models.TimeZone>,ITimeZoneRepository
    {
        public TimeZoneRepository(AppDbContext context) : base(context) { }
    }
}
