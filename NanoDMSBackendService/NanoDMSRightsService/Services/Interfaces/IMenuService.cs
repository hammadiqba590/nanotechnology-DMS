using NanoDMSRightsService.DTO.Menu;
using NanoDMSRightsService.Models;

namespace NanoDMSRightsService.Services.Interfaces
{
    public interface IMenuService
    {
        Task<Menu> CreateAsync(MenuCreateDto dto,Guid userId);
        Task<List<MenuTreeDto>> GetMenuTreeByRolesAsync(List<Guid> roleIds);
        Task<List<Menu>> GetAllAsync();
    }
}
