using NanoDMSRightsService.Data;
using NanoDMSRightsService.Models;
using NanoDMSRightsService.Repositories.Interfaces;

namespace NanoDMSRightsService.Repositories.Implementations
{
    public class MenuRepository : IMenuRepository
    {
        private readonly AppDbContext _context;

        public MenuRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Menu entity)
        => await _context.Menus.AddAsync(entity);

        public void Delete(Menu entity)
        => _context.Menus.Remove(entity);

        public void Update(Menu entity)
        => _context.Menus.Update(entity);
    }
}
