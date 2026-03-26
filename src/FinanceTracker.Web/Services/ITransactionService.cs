using SelfRelianceFinanceTracker.Web.Models;

namespace SelfRelianceFinanceTracker.Web.Services;

public interface ITransactionService
{
    Task<IReadOnlyList<Transaction>> GetRecentForUserAsync(string userId, int take = 10, CancellationToken cancellationToken = default);
}
