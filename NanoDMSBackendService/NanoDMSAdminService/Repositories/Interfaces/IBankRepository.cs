using NanoDMSAdminService.Data;
using NanoDMSAdminService.Filters;
using NanoDMSAdminService.Models;
using System.Linq.Expressions;

namespace NanoDMSAdminService.Repositories.Interfaces
{
    // Repositories/IBankRepository.cs
    public interface IBankRepository
    {
        Task<IEnumerable<Bank>> GetAllAsync();
        Task<Bank?> GetByIdAsync(Guid id);
        Task<Bank?> GetWithCountryAsync(Guid id);
        Task<IEnumerable<Bank>> GetAllByConditionAsync(
    Expression<Func<Bank, bool>> predicate);
        Task<(IEnumerable<Bank> Data, int TotalCount)> GetPagedAsync(BankFilterModel filter);

        Task AddAsync(Bank bank);
        void Update(Bank bank);
        void Delete(Bank bank);
    }

}
