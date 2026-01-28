using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NanoDMSRightsService.Models;

namespace NanoDMSRightsService.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
           : base(options) { }

        public DbSet<Menu> Menus => Set<Menu>();
        public DbSet<RoleMenuPermission> RoleMenuPermissions => Set<RoleMenuPermission>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoleMenuPermission>().ToTable("Role_Menu_Permissions");

            modelBuilder.Entity<AuditLog>().ToTable("Role_Audit_Logs");

            modelBuilder.Entity<RolePermission>().ToTable("Role_Permissions");

            modelBuilder.Entity<Menu>()
            .HasIndex(x => x.Code)
            .IsUnique();

            modelBuilder.Entity<RoleMenuPermission>()
            .HasOne(r => r.Menu)
            .WithMany(r => r.Role_Menu_Permissions) 
                .HasForeignKey(r => r.Menu_Id); 

            modelBuilder.Entity<Menu>()
                .HasMany(x => x.Children)
                .WithOne(x => x.Parent)
                .HasForeignKey(x => x.Parent_Id);

            base.OnModelCreating(modelBuilder);
        }
    }
}
