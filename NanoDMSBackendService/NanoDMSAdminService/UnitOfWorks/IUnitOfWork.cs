using NanoDMSAdminService.Repositories.Interfaces;

namespace NanoDMSAdminService.UnitOfWorks
{
    public interface IUnitOfWork
    {
        ICampaignRepository Campaigns { get; }
        IBankRepository Banks { get; }
        ICountryRepository Countries { get; }
        ICurrencyRepository Currencies { get; }
        IDiscountRuleRepository DiscountRules { get; }
        IDiscountRuleHistoryRepository DiscountRuleHistories { get; }
        Task<int> SaveAsync();
    }
}
