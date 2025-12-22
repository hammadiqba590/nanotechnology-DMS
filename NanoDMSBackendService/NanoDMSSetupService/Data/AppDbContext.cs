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

        public DbSet<Country> Country { get; set; }
        public DbSet<Gender> Gender { get; set; }   
        public DbSet<MaritalStatus> MaritalStatus { get; set; }
        public DbSet<State> State { get; set; }
        public DbSet<Models.TimeZone> TimeZone { get; set; }
        public DbSet<Currency> Currency{ get; set; }
        public DbSet<FinancialYearStartMonth> FinancialYearStartMonth { get; set; }
        public DbSet<StockAccountingMethod> StockAccountingMethod { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // State to Country
            modelBuilder.Entity<State>()
                .HasOne(s => s.Country)          // State has one Country
                .WithMany(c => c.States)         // Country has many States
                .HasForeignKey(s => s.CountryId) // Foreign key in State
                .OnDelete(DeleteBehavior.Cascade); // Optional: Cascade delete

            // City to State
            modelBuilder.Entity<City>()
                .HasOne(c => c.State)           // City has one State
                .WithMany(s => s.Cities)        // State has many Cities
                .HasForeignKey(c => c.StateId)  // Foreign key in City
                .OnDelete(DeleteBehavior.Cascade); // Optional: Cascade delete
        }


    }
}
