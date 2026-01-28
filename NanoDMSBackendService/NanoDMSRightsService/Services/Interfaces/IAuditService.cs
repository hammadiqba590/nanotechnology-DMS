namespace NanoDMSRightsService.Services.Interfaces
{
    public interface IAuditService
    {
        Task LogAsync(Guid userId, string action, string entity, object? oldValue, object? newValue);
    }

}
