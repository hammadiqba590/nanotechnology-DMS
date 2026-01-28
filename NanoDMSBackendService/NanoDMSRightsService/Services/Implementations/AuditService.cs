using NanoDMSRightsService.Data;
using NanoDMSRightsService.Models;
using NanoDMSRightsService.Services.Interfaces;
using System.Text.Json;

namespace NanoDMSRightsService.Services.Implementations
{
    public class AuditService : IAuditService
    {
        private readonly AppDbContext _context;

        public AuditService(AppDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(Guid userId, string action, string entity, object? oldValue, object? newValue)
        {
            _context.AuditLogs.Add(new AuditLog
            {
                Id = Guid.NewGuid(),
                User_Id = userId,
                Action = action,
                Entity = entity,
                Old_Value = oldValue != null ? JsonSerializer.Serialize(oldValue) : null,
                New_Value = newValue != null ? JsonSerializer.Serialize(newValue) : null
            });

            await _context.SaveChangesAsync();
        }
    }

}
