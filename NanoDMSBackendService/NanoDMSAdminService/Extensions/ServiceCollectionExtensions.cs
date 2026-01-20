using NanoDMSAdminService.Models;
using NanoDMSAdminService.Repositories.Implementations;
using NanoDMSAdminService.Repositories.Interfaces;
using NanoDMSAdminService.Services.Implementations;
using NanoDMSAdminService.Services.Interfaces;
using NanoDMSAdminService.UnitOfWorks;

namespace NanoDMSAdminService.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>(); 

            // Repositories
            //Master Entry
            services.AddScoped<IBankRepository, BankRepository>();
            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<ICurrencyRepository, CurrencyRepository>();
            services.AddScoped<IDiscountRuleRepository, DiscountRuleRepository>();
            services.AddScoped<IDiscountRuleHistoryRepository, DiscountRuleHistoryRepository>();

            //Campaign
            services.AddScoped<ICampaignRepository, CampaignRepository>();
            services.AddScoped<ICampaignBankRepository, CampaignBankRepository>();
            services.AddScoped<ICampaignCardBinRepository, CampaignCardBinRepository>();

            //Card
            services.AddScoped<ICardBinRepository, CardBinRepository>();
            services.AddScoped<ICardBrandRepository, CardBrandRepository>();
            services.AddScoped<ICardLevelRepository, CardLevelRepository>();
            services.AddScoped<ICardTypeRepository, CardTypeRepository>();

            //PosTerminal
            services.AddScoped<IPosTerminalMasterRepository, PosTerminalMasterRepository>();
            services.AddScoped<IPosTerminalAssignmentRepository, PosTerminalAssignmentRepository>();
            services.AddScoped<IPosTerminalConfigurationRepository, PosTerminalConfigurationRepository>();
            services.AddScoped<IPosTerminalStatusHistoryRepository, PosTerminalStatusHistoryRepository>();

            //Psp
            services.AddScoped<IPspRepository, PspRepository>();
            services.AddScoped<IPspCategoryRepository, PspCategoryRepository>();
            services.AddScoped<IPspCurrencyRepository, PspCurrencyRepository>();
            services.AddScoped<IPspDocumentRepository, PspDocumentRepository>();
            services.AddScoped<IPspPaymentMethodRepository, PspPaymentMethodRepository>();

            // Services

            //Master Entry
            services.AddScoped<IBankService,BankService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<ICurrencyService, CurrencyService>();
            services.AddScoped<IDiscountRuleService, DiscountRuleService>();
            services.AddScoped<IDiscountRuleHistoryService, DiscountRuleHistoryService>();

            //Campaign
            services.AddScoped<ICampaignService,CampaignService>();
            services.AddScoped<ICampaignBankService, CampaignBankService>();
            services.AddScoped<ICampaignCardBinService, CampaignCardBinService>();

            //Card
            services.AddScoped<ICardBinService, CardBinService>();
            services.AddScoped<ICardBrandService, CardBrandService>();
            services.AddScoped<ICardLevelService, CardLevelService>();
            services.AddScoped<ICardTypeService, CardTypeService>();

            //PosTerminal
            services.AddScoped<IPosTerminalMasterService, PosTerminalMasterService>();
            services.AddScoped<IPosTerminalAssignmentService, PosTerminalAssignmentService>();
            services.AddScoped<IPosTerminalConfigurationService, PosTerminalConfigurationService>();
            services.AddScoped<IPosTerminalStatusHistoryService, PosTerminalStatusHistoryService>();

            //Psp
            services.AddScoped<IPspService, PspService>();
            services.AddScoped<IPspCategoryService, PspCategoryService>();
            services.AddScoped<IPspCurrencyService, PspCurrencyService>();
            services.AddScoped<IPspDocumentService, PspDocumentService>();
            services.AddScoped<IPspPaymentMethodService, PspPaymentMethodService>();

            return services;
        }
    }
}
