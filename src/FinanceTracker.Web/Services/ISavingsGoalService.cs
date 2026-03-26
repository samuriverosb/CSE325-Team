using SelfRelianceFinanceTracker.Web.Models;

namespace SelfRelianceFinanceTracker.Web.Services;

public interface ISavingsGoalService
{
    Task<IReadOnlyList<SavingsGoal>> GetForUserAsync(string userId, CancellationToken cancellationToken = default);
}
