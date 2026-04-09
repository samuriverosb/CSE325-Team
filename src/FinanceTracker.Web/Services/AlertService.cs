using Microsoft.EntityFrameworkCore;
using SelfRelianceFinanceTracker.Web.Data;
using SelfRelianceFinanceTracker.Web.Models;

namespace SelfRelianceFinanceTracker.Web.Services;

public class AlertService(ApplicationDbContext dbContext) : IAlertService
{
    public async Task<IReadOnlyList<AlertNotification>> GetAlertsForUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var periodStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var periodEnd = periodStart.AddMonths(1);

        var categories = await dbContext.Categories
            .AsNoTracking()
            .Where(category => category.UserId == userId)
            .OrderBy(category => category.Name)
            .ToListAsync(cancellationToken);

        var monthlyExpenses = await dbContext.Transactions
            .AsNoTracking()
            .Where(transaction => transaction.UserId == userId
                && transaction.Type == TransactionType.Expense
                && transaction.Date >= periodStart
                && transaction.Date < periodEnd)
            .ToListAsync(cancellationToken);

        var goals = await dbContext.SavingsGoals
            .AsNoTracking()
            .Where(goal => goal.UserId == userId)
            .OrderBy(goal => goal.Deadline)
            .ToListAsync(cancellationToken);

        var alerts = new List<AlertNotification>();

        // Alerts stay intentionally lightweight: each one should point to an action the user can take right away.
        foreach (var category in categories.Where(category => category.MonthlyLimit > 0))
        {
            var spent = monthlyExpenses
                .Where(transaction => transaction.CategoryId == category.Id)
                .Sum(transaction => transaction.Amount);

            var usagePercentage = category.MonthlyLimit == 0
                ? 0m
                : (spent / category.MonthlyLimit) * 100m;

            if (usagePercentage >= 100m)
            {
                alerts.Add(new AlertNotification
                {
                    Title = $"{category.Name} is over budget",
                    Message = $"You spent ${spent:N2} against a monthly limit of ${category.MonthlyLimit:N2}.",
                    Area = "Budget",
                    SuggestedAction = "Review categories",
                    SuggestedRoute = "/categories",
                    Severity = AlertSeverity.Danger
                });

                continue;
            }

            if (usagePercentage >= 90m)
            {
                alerts.Add(new AlertNotification
                {
                    Title = $"{category.Name} is close to its limit",
                    Message = $"This category already used {usagePercentage:N0}% of the monthly budget.",
                    Area = "Budget",
                    SuggestedAction = "Open reports",
                    SuggestedRoute = "/reports",
                    Severity = AlertSeverity.Warning
                });
            }
        }

        foreach (var goal in goals.Where(goal => goal.CurrentAmount < goal.TargetAmount))
        {
            var daysRemaining = (goal.Deadline.Date - DateTime.Today).Days;

            if (daysRemaining < 0)
            {
                alerts.Add(new AlertNotification
                {
                    Title = $"{goal.GoalName} is past due",
                    Message = $"The goal deadline passed on {goal.Deadline:yyyy-MM-dd} and it is still incomplete.",
                    Area = "Savings",
                    SuggestedAction = "Update savings goals",
                    SuggestedRoute = "/savings-goals",
                    Severity = AlertSeverity.Danger
                });

                continue;
            }

            if (daysRemaining <= 14)
            {
                alerts.Add(new AlertNotification
                {
                    Title = $"{goal.GoalName} is due soon",
                    Message = $"The deadline is in {daysRemaining} day{(daysRemaining == 1 ? string.Empty : "s")}.",
                    Area = "Savings",
                    SuggestedAction = "Check savings goals",
                    SuggestedRoute = "/savings-goals",
                    Severity = AlertSeverity.Warning
                });
            }
        }

        if (alerts.Count == 0)
        {
            alerts.Add(new AlertNotification
            {
                Title = "No active alerts",
                Message = "Your current budget usage and savings goals do not need attention right now.",
                Area = "Overview",
                SuggestedAction = "Open dashboard",
                SuggestedRoute = "/dashboard",
                Severity = AlertSeverity.Info
            });
        }

        return alerts;
    }
}
