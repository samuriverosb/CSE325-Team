using SelfRelianceFinanceTracker.Web.Models;

namespace SelfRelianceFinanceTracker.Web.Services;

public interface ITransactionService
{
    Task<IReadOnlyList<Transaction>> GetRecentForUserAsync(string userId, int take = 10, CancellationToken cancellationToken = default);
    Task CreateAsync(Transaction transaction, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task UpdateAsync(Transaction transaction, CancellationToken cancellationToken = default);
}
