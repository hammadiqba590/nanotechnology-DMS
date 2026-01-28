using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using NanoDMSRightsService.Data;
using NanoDMSRightsService.DTO.Menu;
using NanoDMSRightsService.Models;
using NanoDMSRightsService.Services.Interfaces;
using NanoDMSRightsService.UnitOfWorks;

namespace NanoDMSRightsService.Services.Implementations
{
    public class MenuService : IMenuService
    {
        private readonly IUnitOfWork _uow;
        private readonly IAuditService _audit;
        private readonly IDistributedCache _cache;
        private readonly AppDbContext _context;

        public MenuService(
            IUnitOfWork uow,
            IDistributedCache cache,
            IAuditService audit,
            AppDbContext context)
        {
            _uow = uow;
            _cache = cache;
            _audit = audit;
            _context = context;
        }

        // -------------------- CREATE MENU --------------------
        public async Task<Menu> CreateAsync(MenuCreateDto dto, Guid performedByUserId)
        {
            var menu = new Menu
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Code = dto.Code,
                Route = dto.Route,
                Icon = dto.Icon,
                Order = dto.Order,
                Parent_Id = dto.Parent_Id,
                Deleted = false,
                Published = true,
                Create_Date = DateTime.UtcNow,
                Create_User = performedByUserId,
                Business_Id = Guid.Empty,
                BusinessLocation_Id = Guid.Empty,
                Is_Active = true,
                RecordStatus = Blocks.RecordStatus.Active,
            };

            await _uow.Menus.AddAsync(menu);
            await _uow.SaveAsync();

            await _audit.LogAsync(
                performedByUserId,
                "CREATE",
                "Menu",
                null,
                menu
            );

            return menu;
        }

        // -------------------- MENU TREE BY USER --------------------
        public async Task<List<MenuTreeDto>> GetMenuTreeByRolesAsync(List<Guid> roleIds)
        {
            var flatMenus = await _context.RoleMenuPermissions
                .Where(rmp => roleIds.Contains(rmp.Role_Id))
                .Where(rmp => rmp.Menu.Is_Active && !rmp.Menu.Deleted)
                .Select(rmp => new
                {
                    rmp.Menu.Id,
                    rmp.Menu.Name,
                    rmp.Menu.Route,
                    rmp.Menu.Icon,
                    ParentId = rmp.Menu.Parent_Id == Guid.Empty
                               ? (Guid?)null
                               : rmp.Menu.Parent_Id,
                    rmp.Menu.Order,
                    rmp.Permissions
                })
                .AsNoTracking()
                .ToListAsync();

            var groupedMenus = flatMenus
                .GroupBy(m => m.Id)
                .Select(g => new
                {
                    MenuId = g.Key,
                    g.First().Name,
                    g.First().Route,
                    g.First().Icon,
                    ParentId = (Guid?)g.First().ParentId, // 🔥 force Guid?
                    g.First().Order,
                    Permissions = g.Select(x => x.Permissions).Distinct().ToList()
                })
                .ToList();

            var menuIds = groupedMenus.Select(m => m.MenuId).ToHashSet();

            var normalizedMenus = groupedMenus.Select(m => new
            {
                m.MenuId,
                m.Name,
                m.Route,
                m.Icon,
                ParentId = m.ParentId.HasValue && menuIds.Contains(m.ParentId.Value)
                           ? m.ParentId
                           : null,
                m.Order,
                m.Permissions
            }).ToList();

            var lookup = normalizedMenus.ToLookup(m => m.ParentId);

            List<MenuTreeDto> Build(Guid? parentId)
            {
                return lookup[parentId]
                    .OrderBy(m => m.Order)
                    .Select(m => new MenuTreeDto
                    {
                        Id = m.MenuId,
                        Name = m.Name,
                        Route = m.Route,
                        Icon = m.Icon,
                        Permissions = m.Permissions,
                        Children = Build((Guid?)m.MenuId) // 🔥 cast matters
                    })
                    .ToList();
            }

            return Build(null);
        }

        // -------------------- ALL MENUS --------------------
        public async Task<List<Menu>> GetAllAsync()
        {
            return await _context.Menus
                .Where(x => !x.Deleted && x.Is_Active)
                .OrderBy(x => x.Order)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
