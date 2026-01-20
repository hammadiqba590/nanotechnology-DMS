using Microsoft.EntityFrameworkCore.Storage;
using NanoDMSAdminService.Repositories.Interfaces;

namespace NanoDMSAdminService.UnitOfWorks
{
    public interface IUnitOfWork
    {
        ICampaignRepository Campaigns { get; }
        ICampaignBankRepository CampaignBanks { get; }
        ICampaignCardBinRepository CampaignCardBins { get; }
        ICardBinRepository CardBins { get; }
        ICardBrandRepository CardBrands { get; }
        ICardLevelRepository CardLevels { get; }
        ICardTypeRepository CardTypes { get; }
        IPosTerminalMasterRepository PosTerminalMasters { get; }
        IPosTerminalAssignmentRepository PosTerminalAssignments { get; }
        IPosTerminalConfigurationRepository PosTerminalConfigurations { get; }
        IPosTerminalStatusHistoryRepository PosTerminalStatusHistories { get; }
        IPspRepository Psps { get; }
        IPspCategoryRepository PspCategories { get; }
        IPspCurrencyRepository PspCurrencies { get; }
        IPspDocumentRepository PspDocuments { get; }
        IPspPaymentMethodRepository PspPaymentMethods { get; }
        IBankRepository Banks { get; }
        ICountryRepository Countries { get; }
        ICurrencyRepository Currencies { get; }
        IDiscountRuleRepository DiscountRules { get; }
        IDiscountRuleHistoryRepository DiscountRuleHistories { get; }
        Task ExecuteInTransactionAsync(Func<Task> action);
        Task<int> SaveAsync();
    }
}
