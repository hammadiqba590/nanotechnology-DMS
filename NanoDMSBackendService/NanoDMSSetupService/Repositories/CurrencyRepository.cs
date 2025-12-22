using NanoDMSSetupService.Data;
using NanoDMSSetupService.Models;

namespace NanoDMSSetupService.Repositories
{
    public class CurrencyRepository : Repository<Currency>, ICurrencyRepository
    {
        public CurrencyRepository(AppDbContext context) : base(context) { }
    }
}
