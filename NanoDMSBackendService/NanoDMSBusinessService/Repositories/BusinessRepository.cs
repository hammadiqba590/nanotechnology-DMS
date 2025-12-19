using NanoDMSBusinessService.Data;
using NanoDMSBusinessService.Models;

namespace NanoDMSBusinessService.Repositories
{
    public class BusinessRepository : Repository<Business>, IBusinessRepository
    {
        public BusinessRepository(AppDbContext context) : base(context) { }

        

    }
}
