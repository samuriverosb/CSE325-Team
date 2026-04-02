using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SelfRelianceFinanceTracker.Web.Data;
using SelfRelianceFinanceTracker.Web.Models;

namespace SelfRelianceFinanceTracker.Web.Services;

public class DashboardService(ApplicationDbContext dbContext) : IDashboardService
{
    public async Task<DashboardSummary> GetCurrentMonthSummaryAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var end = start.AddMonths(1);

            var monthTransactions = dbContext.Transactions
                .AsNoTracking()
                .Where(t => t.UserId == userId && t.Date >= start && t.Date < end);

            var income = await monthTransactions
                .Where(t => t.Type == TransactionType.Income)
                .SumAsync(t => (decimal?)t.Amount, cancellationToken) ?? 0m;

            var expenses = await monthTransactions
                .Where(t => t.Type == TransactionType.Expense)
                .SumAsync(t => (decimal?)t.Amount, cancellationToken) ?? 0m;

            var activeGoals = await dbContext.SavingsGoals
                .AsNoTracking()
                .CountAsync(g => g.UserId == userId && g.CurrentAmount < g.TargetAmount, cancellationToken);

            return new DashboardSummary
            {
                MonthlyIncome = income,
                MonthlyExpenses = expenses,
                ActiveSavingsGoals = activeGoals
            };
        }
        catch (SqliteException)
        {
            return new DashboardSummary();
        }
    }

    public async Task<IReadOnlyList<CategoryBudgetSummary>> GetCategoryBudgetsAsync(string userId, CancellationToken cancellationToken = default)
    {
        var start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        var end = start.AddMonths(1);

        var categories = await dbContext.Categories
            .Where(c => c.UserId == userId)
            .ToListAsync(cancellationToken);

        var transactions = await dbContext.Transactions
            .Where(t => t.UserId == userId && t.Date >= start && t.Date < end && t.Type == TransactionType.Expense)
            .ToListAsync(cancellationToken);

        var result = categories.Select(c =>
        {
            var spent = transactions
                .Where(t => t.CategoryId == c.Id)
                .Sum(t => t.Amount);

            return new CategoryBudgetSummary
            {
                CategoryName = c.Name,
                Limit = c.MonthlyLimit,
                Spent = spent
            };
        }).ToList();

        return result;
    }
}
