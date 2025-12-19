using NanoDMSBusinessService.Data;
using NanoDMSBusinessService.Models;

namespace NanoDMSBusinessService.Repositories
{
    public interface IBusinessConfigRepository : IRepository<BusinessConfig>
    {
        Task<Dictionary<string, object>> GetConfigValuesAsync(IEnumerable<string> keys);
    }
}
