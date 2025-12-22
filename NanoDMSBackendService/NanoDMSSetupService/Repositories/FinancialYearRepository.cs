using NanoDMSSetupService.Data;
using NanoDMSSetupService.Models;

namespace NanoDMSSetupService.Repositories
{
    public class FinancialYearRepository : Repository<FinancialYearStartMonth>, IFinancialYearRepository
    {

        public FinancialYearRepository(AppDbContext context) : base(context) { }

    }
}
