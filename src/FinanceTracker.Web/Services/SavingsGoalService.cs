using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SelfRelianceFinanceTracker.Web.Data;
using SelfRelianceFinanceTracker.Web.Models;

namespace SelfRelianceFinanceTracker.Web.Services;

public class SavingsGoalService(ApplicationDbContext dbContext) : ISavingsGoalService
{
    public async Task<IReadOnlyList<SavingsGoal>> GetForUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await dbContext.SavingsGoals
                .Where(g => g.UserId == userId)
                .OrderBy(g => g.Deadline)
                .ThenBy(g => g.GoalName)
                .ToListAsync(cancellationToken);
        }
        catch (SqliteException)
        {
            return [];
        }
    }
}
