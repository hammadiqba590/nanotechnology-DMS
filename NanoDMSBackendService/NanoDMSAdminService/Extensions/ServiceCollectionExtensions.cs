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
            services.AddScoped<IUnitOfWork, UnitOfWork>(); // Use fully qualified name if UnitOfWork is a class inside UnitOfWork namespace

            // Repositories
            services.AddScoped<IBankRepository, BankRepository>();
            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<ICurrencyRepository, CurrencyRepository>();
            services.AddScoped<IDiscountRuleRepository, DiscountRuleRepository>();
            services.AddScoped<IDiscountRuleHistoryRepository, DiscountRuleHistoryRepository>();
            services.AddScoped<ICampaignRepository, CampaignRepository>();

            // Services
            services.AddScoped<IBankService,BankService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<ICurrencyService, CurrencyService>();
            services.AddScoped<IDiscountRuleService, DiscountRuleService>();
            services.AddScoped<IDiscountRuleHistoryService, DiscountRuleHistoryService>();
            services.AddScoped<CampaignService>();

            return services;
        }
    }
}
