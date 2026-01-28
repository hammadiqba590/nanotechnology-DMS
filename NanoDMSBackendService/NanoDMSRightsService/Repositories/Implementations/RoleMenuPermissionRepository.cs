using NanoDMSRightsService.Data;
using NanoDMSRightsService.Models;
using NanoDMSRightsService.Repositories.Interfaces;

namespace NanoDMSRightsService.Repositories.Implementations
{
    public class RoleMenuPermissionRepository : IRoleMenuPermissionRepository
    {
        private readonly AppDbContext _context;

        public RoleMenuPermissionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(RoleMenuPermission entity)
        => await _context.RoleMenuPermissions.AddAsync(entity);

        public void Delete(RoleMenuPermission entity)
        => _context.RoleMenuPermissions.Remove(entity);

        public void Update(RoleMenuPermission entity)
        => _context.RoleMenuPermissions.Update(entity);
    }
}
