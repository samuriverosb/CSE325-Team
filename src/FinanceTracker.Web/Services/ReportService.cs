using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SelfRelianceFinanceTracker.Web.Data;
using SelfRelianceFinanceTracker.Web.Models;

namespace SelfRelianceFinanceTracker.Web.Services;

public class ReportService(ApplicationDbContext dbContext) : IReportService
{
    public async Task<MonthlyReport> GetMonthlyReportAsync(string userId, CancellationToken cancellationToken = default)
    {
        var periodStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var periodEnd = periodStart.AddMonths(1);

        try
        {
            var monthTransactions = await dbContext.Transactions
                .AsNoTracking()
                .Include(t => t.Category)
                .Where(t => t.UserId == userId && t.Date >= periodStart && t.Date < periodEnd)
                .OrderByDescending(t => t.Date)
                .ThenByDescending(t => t.Id)
                .ToListAsync(cancellationToken);

            var categories = await dbContext.Categories
                .AsNoTracking()
                .Where(c => c.UserId == userId)
                .OrderBy(c => c.Name)
                .ToListAsync(cancellationToken);

            var goals = await dbContext.SavingsGoals
                .AsNoTracking()
                .Where(g => g.UserId == userId)
                .OrderBy(g => g.Deadline)
                .ThenBy(g => g.GoalName)
                .ToListAsync(cancellationToken);

            var monthlyIncome = monthTransactions
                .Where(t => t.Type == TransactionType.Income)
                .Sum(t => t.Amount);

            var monthlyExpenses = monthTransactions
                .Where(t => t.Type == TransactionType.Expense)
                .Sum(t => t.Amount);

            var expenseTransactions = monthTransactions
                .Where(t => t.Type == TransactionType.Expense)
                .ToList();

            var categoryBreakdowns = BuildCategoryBreakdowns(categories, expenseTransactions, monthlyExpenses);
            var goalProgressItems = goals
                .Select(goal => new GoalProgressReportItem
                {
                    GoalName = goal.GoalName,
                    CurrentAmount = goal.CurrentAmount,
                    TargetAmount = goal.TargetAmount,
                    Deadline = goal.Deadline.Date
                })
                .ToList();

            return new MonthlyReport
            {
                PeriodLabel = periodStart.ToString("MMMM yyyy"),
                MonthlyIncome = monthlyIncome,
                MonthlyExpenses = monthlyExpenses,
                CategoryBreakdowns = categoryBreakdowns,
                GoalProgressItems = goalProgressItems,
                RecentTransactions = monthTransactions.Take(10).ToList(),
                SpendingInsights = BuildSpendingInsights(monthlyIncome, monthlyExpenses, categoryBreakdowns, goalProgressItems, monthTransactions.Count),
                CashFlowPoints = await BuildCashFlowPointsAsync(userId, periodStart, cancellationToken),
                CategorySharePoints = categoryBreakdowns
                    .Where(item => item.Spent > 0)
                    .OrderByDescending(item => item.Spent)
                    .Take(5)
                    .Select(item => new ReportChartPoint
                    {
                        Label = item.CategoryName,
                        Value = item.Spent,
                        Percentage = item.ShareOfExpenses
                    })
                    .ToList()
            };
        }
        catch (SqliteException)
        {
            return new MonthlyReport
            {
                PeriodLabel = periodStart.ToString("MMMM yyyy")
            };
        }
    }

    private static IReadOnlyList<ReportCategoryBreakdown> BuildCategoryBreakdowns(
        IReadOnlyList<Category> categories,
        IReadOnlyList<Transaction> expenseTransactions,
        decimal totalExpenses)
    {
        return categories
            .Select(category =>
            {
                var categoryTransactions = expenseTransactions
                    .Where(transaction => transaction.CategoryId == category.Id)
                    .ToList();

                var spent = categoryTransactions.Sum(transaction => transaction.Amount);
                return new ReportCategoryBreakdown
                {
                    CategoryName = category.Name,
                    BudgetLimit = category.MonthlyLimit,
                    Spent = spent,
                    ShareOfExpenses = totalExpenses <= 0 ? 0 : Math.Round((spent / totalExpenses) * 100m, 1),
                    TransactionCount = categoryTransactions.Count
                };
            })
            .OrderByDescending(item => item.Spent)
            .ThenBy(item => item.CategoryName)
            .ToList();
    }

    private async Task<IReadOnlyList<ReportChartPoint>> BuildCashFlowPointsAsync(
        string userId,
        DateTime currentMonthStart,
        CancellationToken cancellationToken)
    {
        var chartStart = currentMonthStart.AddMonths(-5);
        var chartEnd = currentMonthStart.AddMonths(1);

        var chartTransactions = await dbContext.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId && t.Date >= chartStart && t.Date < chartEnd)
            .ToListAsync(cancellationToken);

        var rawPoints = Enumerable.Range(0, 6)
            .Select(offset =>
            {
                var monthStart = chartStart.AddMonths(offset);
                var monthEnd = monthStart.AddMonths(1);
                var monthTransactions = chartTransactions
                    .Where(transaction => transaction.Date >= monthStart && transaction.Date < monthEnd)
                    .ToList();

                var income = monthTransactions
                    .Where(transaction => transaction.Type == TransactionType.Income)
                    .Sum(transaction => transaction.Amount);

                var expenses = monthTransactions
                    .Where(transaction => transaction.Type == TransactionType.Expense)
                    .Sum(transaction => transaction.Amount);

                return new ReportChartPoint
                {
                    Label = monthStart.ToString("MMM"),
                    Value = income - expenses
                };
            })
            .ToList();

        // Normalize heights once so the Razor page only renders percentages.
        var maxMagnitude = rawPoints
            .Select(point => Math.Abs(point.Value))
            .DefaultIfEmpty(0m)
            .Max();

        return rawPoints
            .Select(point => new ReportChartPoint
            {
                Label = point.Label,
                Value = point.Value,
                Percentage = maxMagnitude <= 0 ? 0 : Math.Round((Math.Abs(point.Value) / maxMagnitude) * 100m, 1)
            })
            .ToList();
    }

    private static IReadOnlyList<SpendingInsight> BuildSpendingInsights(
        decimal monthlyIncome,
        decimal monthlyExpenses,
        IReadOnlyList<ReportCategoryBreakdown> categoryBreakdowns,
        IReadOnlyList<GoalProgressReportItem> goalProgressItems,
        int transactionCount)
    {
        var insights = new List<SpendingInsight>();

        // Keep the insight copy readable and action-oriented for the checkpoint deliverable.
        if (transactionCount == 0)
        {
            insights.Add(new SpendingInsight
            {
                Title = "No spending data recorded yet",
                Description = "Add a few transactions to unlock category trends, budget behavior, and cash flow patterns.",
                Tone = "info"
            });

            return insights;
        }

        if (monthlyIncome > 0 && monthlyExpenses > monthlyIncome)
        {
            insights.Add(new SpendingInsight
            {
                Title = "Expenses outpaced income this month",
                Description = $"Expenses reached ${monthlyExpenses:N2} versus ${monthlyIncome:N2} in income, which puts the monthly balance under pressure.",
                Tone = "warning"
            });
        }
        else if (monthlyIncome > 0)
        {
            insights.Add(new SpendingInsight
            {
                Title = "Monthly cash flow is currently positive",
                Description = $"Income is covering expenses with ${(monthlyIncome - monthlyExpenses):N2} left after this month's recorded activity.",
                Tone = "success"
            });
        }

        var largestCategory = categoryBreakdowns.FirstOrDefault(item => item.Spent > 0);
        if (largestCategory is not null)
        {
            insights.Add(new SpendingInsight
            {
                Title = $"{largestCategory.CategoryName} is the largest expense category",
                Description = $"It accounts for {largestCategory.ShareOfExpenses:N1}% of recorded spending and totals ${largestCategory.Spent:N2} so far.",
                Tone = largestCategory.UtilizationPercentage >= 75 ? "warning" : "info"
            });
        }

        var overLimitCount = categoryBreakdowns.Count(item => item.BudgetLimit > 0 && item.Spent >= item.BudgetLimit);
        if (overLimitCount > 0)
        {
            insights.Add(new SpendingInsight
            {
                Title = "Some category budgets are already over the limit",
                Description = $"{overLimitCount} categor{(overLimitCount == 1 ? "y is" : "ies are")} above the configured monthly budget and should be reviewed.",
                Tone = "warning"
            });
        }

        var strongestGoal = goalProgressItems
            .Where(item => item.TargetAmount > 0)
            .OrderByDescending(item => item.ProgressPercentage)
            .FirstOrDefault();

        if (strongestGoal is not null)
        {
            insights.Add(new SpendingInsight
            {
                Title = $"{strongestGoal.GoalName} is the most advanced savings goal",
                Description = $"Progress is at {strongestGoal.ProgressPercentage:N1}% with ${strongestGoal.RemainingAmount:N2} still needed to finish it.",
                Tone = strongestGoal.ProgressPercentage >= 75 ? "success" : "info"
            });
        }

        return insights.Take(4).ToList();
    }
}
