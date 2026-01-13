using NanoDMSAdminService.Data;
using NanoDMSAdminService.Models;
using NanoDMSAdminService.Repositories;
using NanoDMSAdminService.Repositories.Implementations;
using NanoDMSAdminService.Repositories.Interfaces;
using NanoDMSAdminService.Services.Implementations;

namespace NanoDMSAdminService.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Campaigns = new CampaignRepository(context);
            CampaignBanks = new CampaignBankRepository(context);
            CampaignCardBins = new CampaignCardBinRepository(context);
            CardBins = new CardBinRepository(context);
            CardBrands = new CardBrandRepository(context);
            CardLevels = new CardLevelRepository(context);
            CardTypes = new CardTypeRepository(context);
            PosTerminalMasters = new PosTerminalMasterRepository(context);
            PosTerminalAssignments = new PosTerminalAssignmentRepository(context);
            PosTerminalConfigurations = new PosTerminalConfigurationRepository(context);
            PosTerminalStatusHistories = new PosTerminalStatusHistoryRepository(context);
            Psps = new PspRepository(context);
            PspCategories = new PspCategoryRepository(context);
            PspCurrencies =  new PspCurrencyRepository(context);
            PspDocuments =  new PspDocumentRepository(context);
            PspPaymentMethods = new PspPaymentMethodRepository(context);
            Banks = new BankRepository(context);
            Countries = new CountryRepository(context);
            Currencies = new CurrencyRepository(context);
            DiscountRules = new DiscountRuleRepository(context);
            DiscountRuleHistories = new DiscountRuleHistoryRepository(context);
        }

        public ICampaignRepository Campaigns { get; }
        public ICampaignBankRepository CampaignBanks { get;}
        public ICampaignCardBinRepository CampaignCardBins { get;}
        public ICardBinRepository CardBins { get; }
        public ICardBrandRepository CardBrands { get;}
        public ICardLevelRepository CardLevels { get;}
        public ICardTypeRepository CardTypes { get;}
        public IPosTerminalMasterRepository PosTerminalMasters { get; }
        public IPosTerminalAssignmentRepository PosTerminalAssignments { get;}
        public IPosTerminalConfigurationRepository PosTerminalConfigurations { get;}
        public IPosTerminalStatusHistoryRepository PosTerminalStatusHistories { get;}
        public IPspRepository Psps { get;}
        public IPspCategoryRepository PspCategories { get; }
        public IPspCurrencyRepository PspCurrencies { get;}
        public IPspDocumentRepository PspDocuments { get;}
        public IPspPaymentMethodRepository PspPaymentMethods { get;}
        public IBankRepository Banks { get; }
        public ICountryRepository Countries { get; }
        public ICurrencyRepository Currencies { get;}
        public IDiscountRuleRepository DiscountRules { get; }
        public IDiscountRuleHistoryRepository DiscountRuleHistories { get;}
        
        public async Task<int> SaveAsync()
            => await _context.SaveChangesAsync();
    }


}
