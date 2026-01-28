using NanoDMSRightsService.Repositories.Implementations;
using NanoDMSRightsService.Repositories.Interfaces;
using NanoDMSRightsService.Services.Implementations;
using NanoDMSRightsService.Services.Interfaces;
using NanoDMSRightsService.UnitOfWorks;

namespace NanoDMSRightsService.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Repositories

            services.AddScoped<IMenuRepository, MenuRepository>();
            services.AddScoped<IRoleMenuPermissionRepository, RoleMenuPermissionRepository>();


            //Services

            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<IRightsService, RightsService>();
            services.AddScoped<IAuditService, AuditService>();
            services.AddScoped<IRightsCacheService, RightsCacheService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
