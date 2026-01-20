using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NanoDMSAdminService.Blocks;
using NanoDMSAdminService.Models;

namespace NanoDMSAdminService.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        // ========================
        // DbSets
        // ========================

        public DbSet<Bank> Banks { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<CampaignBank> CampaignBanks { get; set; }
        public DbSet<CampaignCardBin> CampaignCardBins { get; set; }

        public DbSet<CardBin> CardBins { get; set; }
        public DbSet<CardBrand> CardBrands { get; set; }
        public DbSet<CardType> CardTypes { get; set; }
        public DbSet<CardLevel> CardLevels { get; set; }

        public DbSet<Country> Countries { get; set; }
        public DbSet<Currency> Currencies { get; set; }

        public DbSet<DiscountRule> DiscountRules { get; set; }
        public DbSet<DiscountRuleHistory> DiscountRuleHistories { get; set; }

        public DbSet<PosTerminalMaster> PosTerminalMasters { get; set; }
        public DbSet<PosTerminalAssignment> PosTerminalAssignments { get; set; }
        public DbSet<PosTerminalConfiguration> PosTerminalConfigurations { get; set; }
        public DbSet<PosTerminalStatusHistory> PosTerminalStatusHistories { get; set; }

        public DbSet<Psp> Psps { get; set; }
        public DbSet<PspCategory> PspCategories { get; set; }
        public DbSet<PspCurrency> PspCurrencies { get; set; }
        public DbSet<PspDocument> PspDocuments { get; set; }
        public DbSet<PspPaymentMethod> PspPaymentMethods { get; set; }

        // ========================
        // Fluent Configurations
        // ========================

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ------------------------
            // Enums 
            // ------------------------

            modelBuilder.Entity<CardBin>().ToTable("Card_Bins");
            modelBuilder.Entity<CardBrand>().ToTable("Card_Brands");
            modelBuilder.Entity<CardType>().ToTable("Card_Types");
            modelBuilder.Entity<CardLevel>().ToTable("Card_Levels");

            modelBuilder.Entity<CampaignBank>().ToTable("Campaign_Banks");
            modelBuilder.Entity<CampaignCardBin>().ToTable("Campaign_Card_Bins");

            modelBuilder.Entity<DiscountRule>().ToTable("Discount_Rules");
            modelBuilder.Entity<DiscountRuleHistory>().ToTable("Discount_Rule_Histories");

            modelBuilder.Entity<PosTerminalMaster>().ToTable("Pos_Terminal_Masters");
            modelBuilder.Entity<PosTerminalAssignment>().ToTable("Pos_Terminal_Assignments");
            modelBuilder.Entity<PosTerminalConfiguration>().ToTable("Pos_Terminal_Configurations");
            modelBuilder.Entity<PosTerminalStatusHistory>().ToTable("Pos_Terminal_Status_Histories");

            modelBuilder.Entity<PspCategory>().ToTable("Psp_Categories");
            modelBuilder.Entity<PspCurrency>().ToTable("Psp_Currencies");
            modelBuilder.Entity<PspDocument>().ToTable("Psp_Documents");
            modelBuilder.Entity<PspPaymentMethod>().ToTable("Psp_Payment_Methods");

            modelBuilder.HasPostgresEnum<BudgetLimitTypeStatus>();

            modelBuilder.HasPostgresEnum<CampaginCardBinStatus>();

            modelBuilder.HasPostgresEnum<CampaginStatus>();

            modelBuilder.HasPostgresEnum<ChangeTypeStatus>();

            modelBuilder.HasPostgresEnum<ComplianceStatus>();

            modelBuilder.HasPostgresEnum<DiscountModeStatus>();

            modelBuilder.HasPostgresEnum<DiscountTypeStatus>();

            modelBuilder.HasPostgresEnum<IntegrationTypeStatus>();

            modelBuilder.HasPostgresEnum<LocalInternationalStatus>();

            modelBuilder.HasPostgresEnum<PaymentTypeStatus>();

            modelBuilder.HasPostgresEnum<PspPaymentTypeStatus>();

            modelBuilder.HasPostgresEnum<RecordStatus>();

            modelBuilder.HasPostgresEnum<SettlementFrequencyStatus>();

            modelBuilder.HasPostgresEnum<TaxOnMerchantStatus>();

            modelBuilder.HasPostgresEnum<TerminalHistoryStatus>();

            modelBuilder.Entity<Campaign>(entity =>
            {
                entity.Property(e => e.Budget_Limit_Type)
                      .HasDefaultValue(BudgetLimitTypeStatus.Yearly);
            });
            modelBuilder.Entity<CampaignBank>(entity =>
            {
                entity.Property(e => e.Budget_Limit_Type)
                      .HasDefaultValue(BudgetLimitTypeStatus.Yearly);
            });

            modelBuilder.Entity<DiscountRule>(entity =>
            {
                entity.Property(e => e.Budget_Limit_Type)
                      .HasDefaultValue(BudgetLimitTypeStatus.Yearly);
            });

            modelBuilder.Entity<CampaignCardBin>(entity =>
            {
                entity.Property(e => e.Status)
                      .HasDefaultValue(CampaginCardBinStatus.Active);
            });

            modelBuilder.Entity<Campaign>(entity =>
            {
                entity.Property(e => e.Status)
                      .HasDefaultValue(CampaginStatus.Inactive);
            });

            modelBuilder.Entity<DiscountRuleHistory>(entity =>
            {
                entity.Property(e => e.Change_Type)
                      .HasDefaultValue(ChangeTypeStatus.Insert);
            });

            modelBuilder.Entity<Psp>(entity =>
            {
                entity.Property(e => e.Compliance_Status)
                      .HasDefaultValue(ComplianceStatus.Pending);
            });

            modelBuilder.Entity<CampaignBank>(entity =>
            {
                entity.Property(e => e.Discount_Mode)
                      .HasDefaultValue(DiscountModeStatus.All);
            });

            modelBuilder.Entity<DiscountRule>(entity =>
            {
                entity.Property(e => e.Discount_Type)
                      .HasDefaultValue(DiscountTypeStatus.Percentage);
            });

            modelBuilder.Entity<DiscountRuleHistory>(entity =>
            {
                entity.Property(e => e.Discount_Type)
                      .HasDefaultValue(DiscountTypeStatus.Percentage);
            });

            modelBuilder.Entity<Psp>(entity =>
            {
                entity.Property(e => e.Integration_Type)
                      .HasDefaultValue(IntegrationTypeStatus.Direct);
            });

            modelBuilder.Entity<Psp>(entity =>
            {
                entity.Property(e => e.Settlement_Frequency)
                      .HasDefaultValue(SettlementFrequencyStatus.Daily);
            });

            modelBuilder.Entity<CardBin>(entity =>
            {
                entity.Property(e => e.Local_International)
                      .HasDefaultValue(LocalInternationalStatus.Local);
            });

            modelBuilder.Entity<DiscountRule>(entity =>
            {
                entity.Property(e => e.Payment_Type)
                      .HasDefaultValue(PaymentTypeStatus.All);
            });

            modelBuilder.Entity<DiscountRuleHistory>(entity =>
            {
                entity.Property(e => e.Payment_Type)
                      .HasDefaultValue(PaymentTypeStatus.All);
            });

            modelBuilder.Entity<PspPaymentMethod>(entity =>
            {
                entity.Property(e => e.Payment_Type)
                      .HasDefaultValue(PspPaymentTypeStatus.Card);
            });

            modelBuilder.Entity<CampaignBank>(entity =>
            {
                entity.Property(e => e.Status)
                      .HasDefaultValue(RecordStatus.Active);
            });

            modelBuilder.Entity<CampaignBank>(entity =>
            {
                entity.Property(e => e.Tax_On_Merchant_Share)
                      .HasDefaultValue(TaxOnMerchantStatus.Yes);
            });

            modelBuilder.Entity<PosTerminalStatusHistory>(entity =>
            {
                entity.Property(e => e.Status)
                      .HasDefaultValue(TerminalHistoryStatus.Active);
            });

            // ------------------------
            // Unique Constraints
            // ------------------------

            modelBuilder.Entity<Bank>()
                .HasIndex(x => x.Short_Code)
                .IsUnique();

            modelBuilder.Entity<Bank>()
               .HasIndex(x => x.Name)
               .IsUnique();

            modelBuilder.Entity<Country>()
                .HasIndex(x => x.Iso2)
                .IsUnique();

            modelBuilder.Entity<Country>()
                .HasIndex(x => x.Iso3)
                .IsUnique();

            modelBuilder.Entity<Country>()
                .HasIndex(x => x.Name)
                .IsUnique();

            modelBuilder.Entity<PosTerminalMaster>()
               .HasIndex(x => x.Serial_Number)
               .IsUnique();

            modelBuilder.Entity<PosTerminalAssignment>()
               .HasIndex(x => x.Tid)
               .IsUnique();

            modelBuilder.Entity<PosTerminalConfiguration>()
               .HasIndex(x => x.Config_Key)
               .IsUnique();


            modelBuilder.Entity<CardBin>()
               .HasIndex(x => x.Card_Bin_Value)
               .IsUnique();

            modelBuilder.Entity<Campaign>()
                .HasIndex(x => new { x.Campaign_Name })
                .IsUnique(false);

            modelBuilder.Entity<Currency>()
                .HasIndex(x => x.Code)
                .IsUnique();

            modelBuilder.Entity<PspCategory>()
                .HasIndex(x => x.Name)
                .IsUnique();


            modelBuilder.Entity<Psp>()
                .HasIndex(x => x.Code)
                .IsUnique();

            modelBuilder.Entity<Psp>()
                .HasIndex(x => x.Name)
                .IsUnique();

            modelBuilder.Entity<PspCurrency>()
               .HasIndex(x => new { x.Psp_Id, x.Currency_Id })
               .IsUnique();


            // ------------------------
            // Relationships
            // ------------------------

            modelBuilder.Entity<Bank>()
                .HasOne(x => x.Country)
                .WithMany(x => x.Banks)
                .HasForeignKey(x => x.Country_Id);

            modelBuilder.Entity<Campaign>()
               .HasOne(x => x.Psps)
               .WithMany(x => x.Campaigns)
               .HasForeignKey(x => x.Psp_Id);

            modelBuilder.Entity<CampaignBank>()
                .HasOne(x => x.Campaign)
                .WithMany(x => x.CampaignBanks)
                .HasForeignKey(x => x.Campagin_Id);

            modelBuilder.Entity<CampaignBank>()
                .HasOne(x => x.Bank)
                .WithMany(x => x.Campaign_Banks)
                .HasForeignKey(x => x.Bank_Id);

            modelBuilder.Entity<CampaignCardBin>()
                .HasOne(x => x.Campaign_Bank)
                .WithMany(x => x.Campaign_Card_Bins)
                .HasForeignKey(x => x.Campagin_Bank_Id);

            modelBuilder.Entity<CampaignCardBin>()
                .HasOne(x => x.Campaign)
                .WithMany(x => x.CampaignCardBins)
                .HasForeignKey(x => x.Campagin_Id);

            modelBuilder.Entity<CampaignCardBin>()
                .HasOne(x => x.Card_Bin)
                .WithMany(x => x.Campaign_Card_Bins)
                .HasForeignKey(x => x.Card_Bin_Id);

            modelBuilder.Entity<CardBin>()
                .HasOne(x => x.Card_Brand)
                .WithMany(x => x.Card_Bins)
                .HasForeignKey(x => x.Card_Brand_Id);

            modelBuilder.Entity<CardBin>()
               .HasOne(x => x.Card_Type)
               .WithMany(x => x.Card_Bins)
               .HasForeignKey(x => x.Card_Type_Id);

            modelBuilder.Entity<CardBin>()
               .HasOne(x => x.Card_Level)
               .WithMany(x => x.Card_Bins)
               .HasForeignKey(x => x.Card_Level_Id);

            modelBuilder.Entity<CardBin>()
               .HasOne(x => x.Country)
               .WithMany(x => x.Card_Bins)
               .HasForeignKey(x => x.Country_Id);

            modelBuilder.Entity<Currency>()
               .HasOne(x => x.Country)
               .WithMany(x => x.Currencies)
               .HasForeignKey(x => x.Country_Id);

            modelBuilder.Entity<DiscountRule>()
                .HasOne(x => x.CampaignCardBin)
                .WithMany(x => x.Discount_Rules)
                .HasForeignKey(x => x.Campaign_Card_Bin_Id);

            modelBuilder.Entity<DiscountRuleHistory>()
                .HasOne(x => x.DiscountRule)
                .WithMany(x => x.Discount_Rule_Histories)
                .HasForeignKey(x => x.Discount_Rule_Id);

            modelBuilder.Entity<DiscountRuleHistory>()
                .HasOne(x => x.CampaignCardBin)
                .WithMany(x => x.Discount_Rule_Histories)
                .HasForeignKey(x => x.Campaign_Card_Bin_Id);

            modelBuilder.Entity<PosTerminalAssignment>()
               .HasOne(x => x.Pos_Terminal)
               .WithMany(x => x.PosTerminal_Assignments)
               .HasForeignKey(x => x.PosTerminal_Id);

            modelBuilder.Entity<PosTerminalConfiguration>()
              .HasOne(x => x.Pos_Terminal)
              .WithMany(x => x.PosTerminal_Configurations)
              .HasForeignKey(x => x.Pos_Terminal_Id);

            modelBuilder.Entity<PosTerminalStatusHistory>()
                .HasOne(x => x.Pos_Terminal)
                .WithMany(x => x.PosTerminal_Status_Histories)
                .HasForeignKey(x => x.Pos_Terminal_Id);

            modelBuilder.Entity<Psp>()
                .HasOne(x => x.PspCategory)
                .WithMany(x => x.Psps)
                .HasForeignKey(x => x.Psp_Category_Id);

            modelBuilder.Entity<Psp>()
                .HasOne(x => x.Country)
                .WithMany(x => x.Psps)
                .HasForeignKey(x => x.Country_Id);

            modelBuilder.Entity<PspCurrency>()
                .HasOne(x => x.Psp)
                .WithMany(x => x.PspCurrencies)
                .HasForeignKey(x => x.Psp_Id);

            modelBuilder.Entity<PspCurrency>()
                .HasOne(x => x.Currency)
                .WithMany(x => x.Psp_Currencies)
                .HasForeignKey(x => x.Currency_Id);

            modelBuilder.Entity<PspPaymentMethod>()
                .HasOne(x => x.Psp)
                .WithMany(x => x.PspPaymentMethods)
                .HasForeignKey(x => x.Psp_Id);

            modelBuilder.Entity<PspDocument>()
                .HasOne(x => x.Psp)
                .WithMany(x => x.PspDocuments)
                .HasForeignKey(x => x.Psp_Id);

            
        }
    }

}
