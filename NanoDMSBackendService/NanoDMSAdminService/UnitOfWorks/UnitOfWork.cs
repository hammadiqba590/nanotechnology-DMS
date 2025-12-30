using NanoDMSAdminService.Data;
using NanoDMSAdminService.Repositories.Implementations;
using NanoDMSAdminService.Repositories.Interfaces;

namespace NanoDMSAdminService.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Campaigns = new CampaignRepository(context);
            Banks = new BankRepository(context);
            Countries = new CountryRepository(context);
            Currencies = new CurrencyRepository(context);
            DiscountRules = new DiscountRuleRepository(context);
            DiscountRuleHistories = new DiscountRuleHistoryRepository(context);
        }

        public ICampaignRepository Campaigns { get; }

        public IBankRepository Banks { get; }
        public ICountryRepository Countries { get; }
        public ICurrencyRepository Currencies { get;}
        public IDiscountRuleRepository DiscountRules { get; }
        public IDiscountRuleHistoryRepository DiscountRuleHistories { get;}
        public async Task<int> SaveAsync()
            => await _context.SaveChangesAsync();
    }


}
