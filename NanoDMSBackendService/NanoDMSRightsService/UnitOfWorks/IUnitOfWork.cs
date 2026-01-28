using NanoDMSRightsService.Repositories.Interfaces;

namespace NanoDMSRightsService.UnitOfWorks
{
    public interface IUnitOfWork
    {
        IMenuRepository Menus { get; }
        IRoleMenuPermissionRepository RoleMenuPermissions { get; }

        Task ExecuteInTransactionAsync(Func<Task> action);
        Task<int> SaveAsync();
    }
}
