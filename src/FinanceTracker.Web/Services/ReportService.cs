using Microsoft.EntityFrameworkCore;
using SelfRelianceFinanceTracker.Web.Data;
using SelfRelianceFinanceTracker.Web.Models;

namespace SelfRelianceFinanceTracker.Web.Services;

public class ReportService(ApplicationDbContext dbContext) : IReportService
{
    public async Task<MonthlyReport> GetCurrentMonthReportAsync(string userId, CancellationToken cancellationToken = default)
    {
        var periodStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var periodEnd = periodStart.AddMonths(1);

        var categories = await dbContext.Categories
            .AsNoTracking()
            .Where(category => category.UserId == userId)
            .OrderBy(category => category.Name)
            .ToListAsync(cancellationToken);

        var transactions = await dbContext.Transactions
            .AsNoTracking()
            .Include(transaction => transaction.Category)
            .Where(transaction => transaction.UserId == userId && transaction.Date >= periodStart && transaction.Date < periodEnd)
            .OrderByDescending(transaction => transaction.Date)
            .ThenByDescending(transaction => transaction.Id)
            .ToListAsync(cancellationToken);

        var goals = await dbContext.SavingsGoals
            .AsNoTracking()
            .Where(goal => goal.UserId == userId)
            .OrderBy(goal => goal.Deadline)
            .ThenBy(goal => goal.GoalName)
            .ToListAsync(cancellationToken);

        var totalIncome = transactions
            .Where(transaction => transaction.Type == TransactionType.Income)
            .Sum(transaction => transaction.Amount);

        var totalExpenses = transactions
            .Where(transaction => transaction.Type == TransactionType.Expense)
            .Sum(transaction => transaction.Amount);

        var expenseTransactions = transactions
            .Where(transaction => transaction.Type == TransactionType.Expense)
            .ToList();

        // Keep the report anchored to the active month so its numbers match the dashboard.
        var categoryBreakdown = categories
            .Select(category =>
            {
                var spent = transactions
                    .Where(transaction => transaction.Type == TransactionType.Expense && transaction.CategoryId == category.Id)
                    .Sum(transaction => transaction.Amount);

                return new ReportCategoryBreakdown
                {
                    CategoryName = category.Name,
                    Limit = category.MonthlyLimit,
                    Spent = spent,
                    ExpenseSharePercentage = totalExpenses <= 0m
                        ? 0m
                        : Math.Round((spent / totalExpenses) * 100m, 1)
                };
            })
            .OrderByDescending(category => category.Spent)
            .ToList();

        var goalProgress = goals
            .Select(goal => new GoalProgressReportItem
            {
                GoalName = goal.GoalName,
                CurrentAmount = goal.CurrentAmount,
                TargetAmount = goal.TargetAmount,
                Deadline = goal.Deadline
            })
            .ToList();

        // Convert the monthly numbers into short, human-readable observations the user can scan quickly.
        var insights = BuildInsights(categoryBreakdown, expenseTransactions, totalIncome, totalExpenses, goalProgress);

        var cashFlowChart = BuildCashFlowChart(totalIncome, totalExpenses, totalIncome - totalExpenses);

        return new MonthlyReport
        {
            PeriodLabel = periodStart.ToString("MMMM yyyy"),
            TotalIncome = totalIncome,
            TotalExpenses = totalExpenses,
            TransactionCount = transactions.Count,
            CategoryBreakdown = categoryBreakdown,
            GoalProgress = goalProgress,
            RecentTransactions = transactions.Take(8).ToList(),
            Insights = insights,
            CashFlowChart = cashFlowChart
        };
    }

    private static IReadOnlyList<SpendingInsight> BuildInsights(
        IReadOnlyList<ReportCategoryBreakdown> categoryBreakdown,
        IReadOnlyList<Transaction> expenseTransactions,
        decimal totalIncome,
        decimal totalExpenses,
        IReadOnlyList<GoalProgressReportItem> goalProgress)
    {
        var insights = new List<SpendingInsight>();

        var topCategory = categoryBreakdown.FirstOrDefault(category => category.Spent > 0m);
        insights.Add(topCategory is null
            ? new SpendingInsight
            {
                Title = "Top spending category",
                Value = "No expenses yet",
                Summary = "Once expense transactions are added, this insight will highlight where most of the money went.",
                ToneClass = "text-bg-info"
            }
            : new SpendingInsight
            {
                Title = "Top spending category",
                Value = topCategory.CategoryName,
                Summary = $"{topCategory.CategoryName} absorbed ${topCategory.Spent:N2}, which is {topCategory.ExpenseSharePercentage:N0}% of this month's expenses.",
                ToneClass = topCategory.Status == "Over budget" ? "text-bg-danger" : "text-bg-primary"
            });

        var largestExpense = expenseTransactions
            .OrderByDescending(transaction => transaction.Amount)
            .FirstOrDefault();

        insights.Add(largestExpense is null
            ? new SpendingInsight
            {
                Title = "Largest expense",
                Value = "$0.00",
                Summary = "No expense transactions have been recorded for this month.",
                ToneClass = "text-bg-secondary"
            }
            : new SpendingInsight
            {
                Title = "Largest expense",
                Value = $"${largestExpense.Amount:N2}",
                Summary = $"The highest single expense was {(string.IsNullOrWhiteSpace(largestExpense.Description) ? "an uncategorized purchase" : largestExpense.Description)} on {largestExpense.Date:yyyy-MM-dd}.",
                ToneClass = "text-bg-warning"
            });

        var averageExpense = expenseTransactions.Count == 0
            ? 0m
            : expenseTransactions.Average(transaction => transaction.Amount);

        insights.Add(new SpendingInsight
        {
            Title = "Average expense",
            Value = $"${averageExpense:N2}",
            Summary = expenseTransactions.Count == 0
                ? "Average expense will appear once there are outgoing transactions in the current month."
                : $"Across {expenseTransactions.Count} expense transactions, the typical outgoing amount is ${averageExpense:N2}.",
            ToneClass = "text-bg-info"
        });

        var pressuredCategories = categoryBreakdown.Count(category => category.Status is "Near limit" or "Over budget");
        var activeGoals = goalProgress.Count(goal => goal.Status != "Completed");

        insights.Add(new SpendingInsight
        {
            Title = "Pressure check",
            Value = pressuredCategories == 0 ? "Stable" : pressuredCategories.ToString(),
            Summary = pressuredCategories == 0
                ? $"Spending is still within comfortable limits, and {activeGoals} savings goal(s) remain active."
                : $"{pressuredCategories} category budget(s) need attention while {activeGoals} savings goal(s) are still in progress.",
            ToneClass = pressuredCategories == 0 && totalIncome >= totalExpenses ? "text-bg-success" : "text-bg-danger"
        });

        return insights;
    }

    private static IReadOnlyList<ReportChartPoint> BuildCashFlowChart(decimal totalIncome, decimal totalExpenses, decimal netBalance)
    {
        var scaleBase = new[] { totalIncome, totalExpenses, Math.Abs(netBalance) }
            .Max();

        if (scaleBase <= 0m)
        {
            scaleBase = 1m;
        }

        // Use one shared scale so the bars tell a proportional story without a chart library.
        return
        [
            new ReportChartPoint
            {
                Label = "Income",
                Caption = "Money coming in",
                Amount = totalIncome,
                Percentage = Math.Round((totalIncome / scaleBase) * 100m, 1),
                FillClass = "bg-success"
            },
            new ReportChartPoint
            {
                Label = "Expenses",
                Caption = "Money going out",
                Amount = totalExpenses,
                Percentage = Math.Round((totalExpenses / scaleBase) * 100m, 1),
                FillClass = "bg-danger"
            },
            new ReportChartPoint
            {
                Label = "Net balance",
                Caption = netBalance >= 0m ? "What remains after spending" : "Current deficit",
                Amount = Math.Abs(netBalance),
                Percentage = Math.Round((Math.Abs(netBalance) / scaleBase) * 100m, 1),
                FillClass = netBalance >= 0m ? "bg-primary" : "bg-warning"
            }
        ];
    }
}
