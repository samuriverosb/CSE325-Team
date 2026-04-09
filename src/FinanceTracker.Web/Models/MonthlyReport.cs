namespace SelfRelianceFinanceTracker.Web.Models;

public class MonthlyReport
{
    public string PeriodLabel { get; init; } = string.Empty;
    public decimal TotalIncome { get; init; }
    public decimal TotalExpenses { get; init; }
    public int TransactionCount { get; init; }

    public decimal NetBalance => TotalIncome - TotalExpenses;

    public IReadOnlyList<ReportCategoryBreakdown> CategoryBreakdown { get; init; } = [];
    public IReadOnlyList<GoalProgressReportItem> GoalProgress { get; init; } = [];
    public IReadOnlyList<Transaction> RecentTransactions { get; init; } = [];
    public IReadOnlyList<SpendingInsight> Insights { get; init; } = [];
    public IReadOnlyList<ReportChartPoint> CashFlowChart { get; init; } = [];
}
