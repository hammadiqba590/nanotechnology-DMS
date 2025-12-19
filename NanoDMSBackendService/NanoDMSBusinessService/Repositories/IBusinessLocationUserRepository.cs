using NanoDMSBusinessService.Data;
using NanoDMSBusinessService.Models;

namespace NanoDMSBusinessService.Repositories
{
    public interface IBusinessLocationUserRepository : IRepository<BusinessLocationUser>
    {
        // Add multiple BusinessLocationUsers
        Task AddRangeAsync(List<BusinessLocationUser> businessLocationUsers);

        // Get BusinessLocationUsers by UserId
        Task<IEnumerable<BusinessLocationUser>> GetByUserIdAsync(Guid userId);

        // Delete a specific BusinessLocationUser
        new void Delete(BusinessLocationUser entity); // Use 'new' keyword to explicitly hide the inherited member

        // Save changes to the database
        new Task SaveChangesAsync(); // Use 'new' keyword to explicitly hide the inherited member
    }

}
