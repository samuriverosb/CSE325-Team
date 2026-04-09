namespace SelfRelianceFinanceTracker.Web.Models;

public class MonthlyReport
{
    public string PeriodLabel { get; init; } = string.Empty;
    public decimal MonthlyIncome { get; init; }
    public decimal MonthlyExpenses { get; init; }
    public IReadOnlyList<ReportCategoryBreakdown> CategoryBreakdowns { get; init; } = [];
    public IReadOnlyList<GoalProgressReportItem> GoalProgressItems { get; init; } = [];
    public IReadOnlyList<Transaction> RecentTransactions { get; init; } = [];
    public IReadOnlyList<SpendingInsight> SpendingInsights { get; init; } = [];
    public IReadOnlyList<ReportChartPoint> CashFlowPoints { get; init; } = [];
    public IReadOnlyList<ReportChartPoint> CategorySharePoints { get; init; } = [];

    public decimal NetBalance => MonthlyIncome - MonthlyExpenses;

    public decimal ExpenseRatio =>
        MonthlyIncome <= 0 ? 0 : Math.Round((MonthlyExpenses / MonthlyIncome) * 100m, 1);
}
