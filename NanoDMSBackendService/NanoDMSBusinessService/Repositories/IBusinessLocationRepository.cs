using NanoDMSBusinessService.Data;
using NanoDMSBusinessService.Models;

namespace NanoDMSBusinessService.Repositories
{
    public interface IBusinessLocationRepository:IRepository<BusinessLocation>
    {
        Task<int> GetLocationCountByBusinessIdAsync(Guid businessId);
        Task<BusinessLocation?> GetByIdWithBusinessAsync(Guid id);
        Task<List<BusinessLocation>> GetAllWithBusinessAsync();
    }
}
