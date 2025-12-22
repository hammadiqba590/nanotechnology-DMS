using Microsoft.EntityFrameworkCore;
using NanoDMSSetupService.Data;
using NanoDMSSetupService.Models;

namespace NanoDMSSetupService.Repositories
{
    public class StateRepository : Repository<State>, IStateRepository
    {
        public StateRepository(AppDbContext context) : base(context) { }
        
    }
}
