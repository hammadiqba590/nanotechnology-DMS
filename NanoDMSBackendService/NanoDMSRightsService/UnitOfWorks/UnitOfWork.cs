using Microsoft.EntityFrameworkCore;
using NanoDMSRightsService.Data;
using NanoDMSRightsService.Repositories.Implementations;
using NanoDMSRightsService.Repositories.Interfaces;

namespace NanoDMSRightsService.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Menus = new MenuRepository(context);
            RoleMenuPermissions = new RoleMenuPermissionRepository(context);
           
        }

        public async Task ExecuteInTransactionAsync(Func<Task> action)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    await action();
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        public IMenuRepository Menus { get;}
        public IRoleMenuPermissionRepository RoleMenuPermissions { get; }
        public async Task<int> SaveAsync()
            => await _context.SaveChangesAsync();
    }
}
