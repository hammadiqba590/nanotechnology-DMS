using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NanoDMSBusinessService.Models;

namespace NanoDMSBusinessService.Data
{
    public class AppDbContext : DbContext
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

            modelBuilder.Entity<BusinessLocation>(entity =>
            {
                entity.ToTable("Business_Location");
            });

            modelBuilder.Entity<BusinessConfig>(entity =>
            {
                entity.ToTable("Business_Config");
            });

            modelBuilder.Entity<BusinessLocationUser>(entity =>
            {
                entity.ToTable("Business_Location_User");
            });

            modelBuilder.Entity<BusinessUser>(entity =>
            {
                entity.ToTable("Business_User");
            });

            // Configure the relationship between BusinessLocation and Business
            modelBuilder.Entity<BusinessLocation>()
                .HasOne(bu => bu.Business)
                .WithMany(b => b.BusinessLocations)
                .HasForeignKey(bu => bu.Business_Id)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure the relationship between BusinessUser and Business
            modelBuilder.Entity<BusinessUser>()
                .HasOne(bu => bu.Business)
                .WithMany(b => b.BusinessUsers)
                .HasForeignKey(bu => bu.Business_Id)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure the relationship between BusinessLocationUser and Business
            modelBuilder.Entity<BusinessLocationUser>()
                .HasOne(blu => blu.Business)
                .WithMany(b => b.BusinessLocationUsers) // Add navigation property in Business
                .HasForeignKey(blu => blu.Business_Id)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure the relationship between BusinessLocationUser and BusinessLocation
            modelBuilder.Entity<BusinessLocationUser>()
                .HasOne(blu => blu.Business_Location)
                .WithMany(bl => bl.BusinessLocationUsers) // Add navigation property in BusinessLocation
                .HasForeignKey(blu => blu.Business_Location_Id)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure the relationship between BusinessLocationUser and Business
            modelBuilder.Entity<BusinessConfig>()
                .HasOne(blu => blu.Business)
                .WithMany(b => b.BusinessConfigs) // Add navigation property in Business
                .HasForeignKey(blu => blu.Business_Id)
                .OnDelete(DeleteBehavior.Cascade);


            // Configure the relationship between BusinessLocationUser and BusinessLocation
            modelBuilder.Entity<BusinessConfig>()
                .HasOne(blu => blu.BusinessLocation)
                .WithMany(bl => bl.BusinessConfigs) // Add navigation property in BusinessLocation
                .HasForeignKey(blu => blu.Business_Location_Id)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
