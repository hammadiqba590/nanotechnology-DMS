using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using NanoDMSRightsService.Blocks;
using NanoDMSRightsService.Data;
using NanoDMSRightsService.DTO.Claims;
using NanoDMSRightsService.DTO.Menu;
using NanoDMSRightsService.DTO.RoleMenu;
using NanoDMSRightsService.Models;
using NanoDMSRightsService.Services.Interfaces;
using NanoDMSRightsService.UnitOfWorks;
using System.Text.Json;

namespace NanoDMSRightsService.Services.Implementations
{
    public class RightsService : IRightsService
    {
        private readonly IUnitOfWork _uow;
        private readonly IDistributedCache _cache;
        private readonly IAuditService _audit;
        private readonly AppDbContext _context;

        public RightsService(
            AppDbContext context,
            IUnitOfWork uow,
            IDistributedCache cache,
            IAuditService audit)
        {
            _context = context;
            _uow = uow;
            _cache = cache;
            _audit = audit;
        }

        // -------------------- ASSIGN MENUS TO ROLE --------------------
        public async Task AssignMenusAsync(AssignMenusToRoleDto dto, Guid performedByUserId)
        {
            try
            {
                // 1️⃣ Remove all existing permissions for this role
                var existing = await _context.RoleMenuPermissions
                    .Where(x => x.Role_Id == dto.Role_Id)
                    .ToListAsync();

                if (existing.Any())
                {
                    _context.RoleMenuPermissions.RemoveRange(existing);
                    await _context.SaveChangesAsync(); // flush deletes
                }

                // 2️⃣ Insert each permission as its own row
                var newEntries = dto.Menus.Select(m => new RoleMenuPermission
                {
                    Id = Guid.NewGuid(),
                    Role_Id = dto.Role_Id,
                    Menu_Id = m.Menu_Id,
                    Permissions = m.Permissions,
                    Deleted = false,
                    Published = true,
                    Create_Date = DateTime.UtcNow,
                    Create_User = performedByUserId,
                    Business_Id = Guid.Empty,
                    BusinessLocation_Id = Guid.Empty,
                    Is_Active = true,
                    RecordStatus = Blocks.RecordStatus.Active,
                }).ToList();

                _context.RoleMenuPermissions.AddRange(newEntries);
                await _context.SaveChangesAsync();

                //// 3️⃣ Invalidate cache for all users of this role
                //var affectedUserIds = await _context.UserRoles
                //    .Where(x => x.RoleId == Convert.ToString(dto.Role_Id))
                //    .Select(x => x.UserId)
                //    .Distinct()
                //    .ToListAsync();

                //foreach (var uid in affectedUserIds)
                //    await _cache.RemoveAsync($"rights:claims:{uid}");

                // 4️⃣ Audit log
                await _audit.LogAsync(
                    performedByUserId,
                    "ASSIGN_MENUS",
                    "RoleMenuPermission",
                    existing,
                    newEntries
                );
            }
            catch (Exception ex)
            {

                throw ex.InnerException ?? ex;
            }
        }



        // -------------------- USER CLAIMS --------------------
        public async Task<UserClaimsDto> GetClaimsByRolesAsync(List<Guid> roleIds)
        {
            try
            {
                var data = await _context.RoleMenuPermissions
                .Where(x => roleIds.Contains(x.Role_Id))
                .Include(x => x.Menu)
                .AsNoTracking()
                .ToListAsync();

                return new UserClaimsDto
                {
                    Roles = roleIds.Select(r => r.ToString()).ToList(),

                    Permissions = data
                        .SelectMany(x =>
                            Enum.GetValues<Permissions>()
                                .Where(p => p != Permissions.None && x.Permissions.HasFlag(p))
                                .Select(p => $"{x.Menu.Code}:{p}")
                        )
                        .Distinct()
                        .ToList()
                };
            }
            catch (Exception ex)
            {

                throw ex.InnerException ?? ex;
            }
        }


        // -------------------- MENUS BY USER --------------------
        public async Task<List<MenuDto>> GetMenusByRolesAsync(List<Guid> roleIds)
        {
            try
            {
                var menus = await _context.RoleMenuPermissions
                .Where(x => roleIds.Contains(x.Role_Id))
                .Where(x => x.Menu.Is_Active && !x.Menu.Deleted)
                .Select(x => x.Menu)
                .Distinct()
                .OrderBy(x => x.Order)
                .AsNoTracking()
                .ToListAsync();

                return menus.Select(m => new MenuDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Route = m.Route,
                    Icon = m.Icon,
                    Parent_Id = m.Parent_Id
                }).ToList();
            }
            catch (Exception ex)
            {

                throw ex.InnerException ?? ex;
            }
        }

    }
}
