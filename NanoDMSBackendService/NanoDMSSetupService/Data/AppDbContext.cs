using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NanoDMSSetupService.Models;

namespace NanoDMSSetupService.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<City> City { get; set; }
        public DbSet<Gender> Gender { get; set; }   
        public DbSet<MaritalStatus> MaritalStatus { get; set; }
        public DbSet<State> State { get; set; }
        public DbSet<Models.TimeZone> TimeZone { get; set; }
        public DbSet<FinancialYearStartMonth> FinancialYearStartMonth { get; set; }
        public DbSet<StockAccountingMethod> StockAccountingMethod { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MaritalStatus>(entity =>
            {
                entity.ToTable("Marital_Status");
            });

            modelBuilder.Entity<Models.TimeZone>(entity =>
            {
                entity.ToTable("Time_Zone");
            });

            modelBuilder.Entity<FinancialYearStartMonth>(entity =>
            {
                entity.ToTable("Financial_Year_Start_Month");
            });

            modelBuilder.Entity<StockAccountingMethod>(entity =>
            {
                entity.ToTable("Stock_Accounting_Method");
            });
        }


    }
}
