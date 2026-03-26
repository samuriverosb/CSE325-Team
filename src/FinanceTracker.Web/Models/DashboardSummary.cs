namespace SelfRelianceFinanceTracker.Web.Models;

public class DashboardSummary
{
    public decimal MonthlyIncome { get; init; }
    public decimal MonthlyExpenses { get; init; }
    public decimal RemainingBudget => MonthlyIncome - MonthlyExpenses;
    public int ActiveSavingsGoals { get; init; }
}
