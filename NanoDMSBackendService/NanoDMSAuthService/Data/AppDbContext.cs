using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NanoDMSAuthService.Common;
using NanoDMSAuthService.Models;

namespace NanoDMSAuthService.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Add custom DbSets here if needed
        // For example:
         public DbSet<PasswordHistory> PasswordHistory { get; set; }

         public DbSet<UserProfile> UserProfile { get; set; }

         public DbSet<AuditLogin> AuditLogins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --------------------
            // Identity Tables
            // --------------------
            modelBuilder.Entity<AppUser>().ToTable("Users");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("User_Roles");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("User_Claims");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("User_Logins");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("Role_Claims");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("User_Tokens");

            // --------------------
            // UserProfile
            // --------------------
            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.ToTable("User_Profile");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            // --------------------
            // AuditLogin
            // --------------------
            modelBuilder.Entity<AuditLogin>(entity =>
            {
                entity.ToTable("Audit_Login");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.UserName).IsRequired();
                entity.Property(e => e.Password).IsRequired();
                entity.Property(e => e.IpAddress).HasMaxLength(50);
                entity.Property(e => e.MacAddress).HasMaxLength(50);
                entity.Property(e => e.PcName).HasMaxLength(100);
            });

            // --------------------
            // PasswordHistory
            // --------------------
            modelBuilder.Entity<PasswordHistory>(entity =>
            {
                entity.ToTable("Password_History");
            });
        }




    }
}
