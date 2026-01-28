using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NanoDMSRightsService.Data;
using NanoDMSRightsService.DTO.Permission;
using NanoDMSRightsService.DTO.RoleMenu;
using NanoDMSRightsService.Services.Interfaces;

namespace NanoDMSRightsService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RightsController : ControllerBase
    {
        private readonly IRightsService _service;
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public RightsController(IRightsService service,
            AppDbContext context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _service = service;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize]
        [HttpPost("assign-menus")]
        public async Task<IActionResult> AssignMenus(AssignMenusToRoleDto dto)
        {
            var user = await _userManager.FindByNameAsync(User.Identity!.Name!);
            if (user == null) return Unauthorized();

            await _service.AssignMenusAsync(dto, Guid.Parse(user.Id));

            // Fetch updated menus for this role
            var updatedMenus = await _context.RoleMenuPermissions
                .Where(x => x.Role_Id == dto.Role_Id)
                .Select(x => new
                {
                    x.Menu_Id,
                    x.Permissions
                })
                .ToListAsync();

            
            return Ok(new
            {
                Message = "Menu Assigned Successfully",
                Menus = updatedMenus
            });
        }


        [HttpGet("permissions-by-role-ids")]
        public async Task<IActionResult> GetPermissionsByRoleIds(List<Guid> roleIds)
        {
            var permissions = await _context.RolePermissions
                .Where(rp => roleIds.Contains(rp.RoleId))
                .Select(rp => new PermissionDto
                {
                    Code = rp.Permission.Code
                })
                .Distinct()
                .ToListAsync();

            return Ok(permissions);
        }

        [Authorize]
        [HttpGet("get-claims-by-roles-ids")]
        public async Task<IActionResult> Claims(List<Guid> roleIds)
        {
            return Ok(await _service.GetClaimsByRolesAsync(roleIds));
        }

        [Authorize]
        [HttpGet("get-menus-by-role-ids")]
        public async Task<IActionResult> GetMenus(List<Guid> roleIds)
            => Ok(await _service.GetMenusByRolesAsync(roleIds));


       
    }
}
