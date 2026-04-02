using SelfRelianceFinanceTracker.Web.Models;

namespace SelfRelianceFinanceTracker.Web.Services;

public interface ISavingsGoalService
{
    Task<IReadOnlyList<SavingsGoal>> GetForUserAsync(string userId, CancellationToken cancellationToken = default);
    Task CreateAsync(SavingsGoal goal, CancellationToken cancellationToken = default);
    Task UpdateAsync(SavingsGoal goal, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
