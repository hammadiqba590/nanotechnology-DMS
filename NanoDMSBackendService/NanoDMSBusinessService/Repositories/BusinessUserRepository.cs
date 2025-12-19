using NanoDMSBusinessService.Data;
using NanoDMSBusinessService.Models;

namespace NanoDMSBusinessService.Repositories
{
    public class BusinessUserRepository: Repository<BusinessUser>,IBusinessUserRepository
    {
        public BusinessUserRepository(AppDbContext context) : base(context) { }
    }
}
