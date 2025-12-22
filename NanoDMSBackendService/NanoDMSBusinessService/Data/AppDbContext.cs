using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NanoDMSBusinessService.Models;

namespace NanoDMSBusinessService.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Business> Business { get; set; }
        public DbSet<BusinessLocation> BusinessLocation { get; set; }
        public DbSet<BusinessUser> BusinessUser { get; set; }
        public DbSet<BusinessLocationUser> BusinessLocationUser { get; set; }
        public DbSet<BusinessConfig> BusinessConfigs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the relationship between BusinessLocation and Business
            modelBuilder.Entity<BusinessLocation>()
                .HasOne(bu => bu.Business)
                .WithMany(b => b.BusinessLocations)
                .HasForeignKey(bu => bu.BusinessId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure the relationship between BusinessUser and Business
            modelBuilder.Entity<BusinessUser>()
                .HasOne(bu => bu.Business)
                .WithMany(b => b.BusinessUsers)
                .HasForeignKey(bu => bu.BusinessId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure the relationship between BusinessLocationUser and Business
            modelBuilder.Entity<BusinessLocationUser>()
                .HasOne(blu => blu.Business)
                .WithMany(b => b.BusinessLocationUsers) // Add navigation property in Business
                .HasForeignKey(blu => blu.BusinessId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure the relationship between BusinessLocationUser and BusinessLocation
            modelBuilder.Entity<BusinessLocationUser>()
                .HasOne(blu => blu.BusinessLocation)
                .WithMany(bl => bl.BusinessLocationUsers) // Add navigation property in BusinessLocation
                .HasForeignKey(blu => blu.BusinessLocationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure the relationship between BusinessLocationUser and Business
            modelBuilder.Entity<BusinessConfig>()
                .HasOne(blu => blu.Business)
                .WithMany(b => b.BusinessConfigs) // Add navigation property in Business
                .HasForeignKey(blu => blu.BusinessId)
                .OnDelete(DeleteBehavior.Cascade);


            // Configure the relationship between BusinessLocationUser and BusinessLocation
            modelBuilder.Entity<BusinessConfig>()
                .HasOne(blu => blu.BusinessLocation)
                .WithMany(bl => bl.BusinessConfigs) // Add navigation property in BusinessLocation
                .HasForeignKey(blu => blu.BusinessLocationId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
