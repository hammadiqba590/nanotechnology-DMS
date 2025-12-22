using NanoDMSSetupService.Data;
using NanoDMSSetupService.Models;

namespace NanoDMSSetupService.Repositories
{
    public class StockAccountingRepository: Repository<StockAccountingMethod>,IStockAccountingRepository
    {
        public StockAccountingRepository(AppDbContext context) : base(context) { }
    }
}
