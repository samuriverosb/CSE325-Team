using SelfRelianceFinanceTracker.Web.Models;

namespace SelfRelianceFinanceTracker.Web.Services;

public class AlertService(IReportService reportService) : IAlertService
{
    public async Task<IReadOnlyList<AlertNotification>> GetAlertsAsync(string userId, CancellationToken cancellationToken = default)
    {
        var report = await reportService.GetMonthlyReportAsync(userId, cancellationToken);
        var alerts = new List<AlertNotification>();

        if (report.MonthlyIncome > 0 && report.MonthlyExpenses > report.MonthlyIncome)
        {
            alerts.Add(new AlertNotification
            {
                Severity = AlertSeverity.High,
                Title = "Monthly expenses are higher than income",
                Message = $"Expenses reached ${report.MonthlyExpenses:N2} while income is ${report.MonthlyIncome:N2}.",
                ActionText = "Open reports",
                ActionHref = "/reports"
            });
        }

        foreach (var budget in report.CategoryBreakdowns.Where(item => item.BudgetLimit > 0).OrderByDescending(item => item.UtilizationPercentage))
        {
            if (budget.Spent >= budget.BudgetLimit)
            {
                alerts.Add(new AlertNotification
                {
                    Severity = AlertSeverity.High,
                    Title = $"Budget exceeded: {budget.CategoryName}",
                    Message = $"Spending is ${budget.Spent:N2} against a ${budget.BudgetLimit:N2} limit.",
                    ActionText = "Review transactions",
                    ActionHref = "/transactions"
                });
                continue;
            }

            if (budget.UtilizationPercentage >= 75)
            {
                alerts.Add(new AlertNotification
                {
                    Severity = AlertSeverity.Medium,
                    Title = $"Budget watch: {budget.CategoryName}",
                    Message = $"{budget.UtilizationPercentage:N1}% of the monthly budget has already been used.",
                    ActionText = "Open reports",
                    ActionHref = "/reports"
                });
            }
        }

        foreach (var goal in report.GoalProgressItems.Where(item => item.RemainingAmount > 0))
        {
            if (goal.DaysRemaining < 0)
            {
                alerts.Add(new AlertNotification
                {
                    Severity = AlertSeverity.High,
                    Title = $"Past due goal: {goal.GoalName}",
                    Message = $"The deadline passed on {goal.Deadline:yyyy-MM-dd} and ${goal.RemainingAmount:N2} is still missing.",
                    ActionText = "Open savings goals",
                    ActionHref = "/savings-goals"
                });
                continue;
            }

            if (goal.DaysRemaining <= 30)
            {
                alerts.Add(new AlertNotification
                {
                    Severity = AlertSeverity.Medium,
                    Title = $"Goal deadline approaching: {goal.GoalName}",
                    Message = $"{goal.DaysRemaining} day(s) remain to save ${goal.RemainingAmount:N2}.",
                    ActionText = "Update goal",
                    ActionHref = "/savings-goals"
                });
            }
        }

        if (report.RecentTransactions.Count == 0)
        {
            alerts.Add(new AlertNotification
            {
                Severity = AlertSeverity.Info,
                Title = "No transaction activity recorded",
                Message = "Add at least one transaction so the tracker can generate more useful reporting.",
                ActionText = "Add transaction",
                ActionHref = "/transactions"
            });
        }
        else
        {
            var latestTransactionDate = report.RecentTransactions.Max(item => item.Date.Date);
            var inactiveDays = (DateTime.Today - latestTransactionDate).Days;
            if (inactiveDays >= 14)
            {
                alerts.Add(new AlertNotification
                {
                    Severity = AlertSeverity.Info,
                    Title = "Transaction activity looks stale",
                    Message = $"The latest recorded transaction is from {latestTransactionDate:yyyy-MM-dd}.",
                    ActionText = "Review transactions",
                    ActionHref = "/transactions"
                });
            }
        }

        return alerts
            .OrderByDescending(item => item.Severity)
            .ThenBy(item => item.Title, StringComparer.Ordinal)
            .ToList();
    }
}
