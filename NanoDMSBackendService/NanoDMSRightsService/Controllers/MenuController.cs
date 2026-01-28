using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NanoDMSRightsService.DTO.Menu;
using NanoDMSRightsService.Services.Implementations;
using NanoDMSRightsService.Services.Interfaces;

namespace NanoDMSRightsService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _service;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public MenuController(IMenuService service,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _service = service;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize]
        [HttpPost("create-menus")]
        public async Task<IActionResult> Create(MenuCreateDto dto)
        {
            var user = await _userManager.FindByNameAsync(User.Identity!.Name!);
            if (user == null) return Unauthorized();

            var menu = _service.CreateAsync(dto,Guid.Parse(user.Id));

            return Ok(new
            {
                Message = "Menu created successfully",
                Menu = menu
            });

        }
        [Authorize]
        [HttpGet("get-all-menus")]
        public async Task<IActionResult> GetMenus()
       => Ok(await _service.GetAllAsync());

        [Authorize]
        [HttpGet("get-menu-tree-by-role-ids")]
        public async Task<IActionResult> GetMenuTree(List<Guid> roleIds)
        => Ok(await _service.GetMenuTreeByRolesAsync(roleIds));
    }
}
