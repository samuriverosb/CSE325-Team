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

    public async Task CreateAsync(SavingsGoal goal, CancellationToken cancellationToken = default)
    {
        dbContext.SavingsGoals.Add(goal);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(SavingsGoal goal, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.SavingsGoals.FindAsync([goal.Id], cancellationToken);
        if (existing is null) return;

        existing.GoalName = goal.GoalName;
        existing.TargetAmount = goal.TargetAmount;
        existing.CurrentAmount = goal.CurrentAmount;
        existing.Deadline = goal.Deadline;

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var goal = await dbContext.SavingsGoals.FindAsync([id], cancellationToken);
        if (goal is null) return;

        dbContext.SavingsGoals.Remove(goal);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
