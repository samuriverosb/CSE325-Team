namespace SelfRelianceFinanceTracker.Web.Models;

public class ReportCategoryBreakdown
{
    public string CategoryName { get; init; } = string.Empty;
    public decimal BudgetLimit { get; init; }
    public decimal Spent { get; init; }
    public decimal ShareOfExpenses { get; init; }
    public int TransactionCount { get; init; }

    public decimal RemainingAmount => BudgetLimit - Spent;

    public decimal UtilizationPercentage =>
        BudgetLimit <= 0 ? 0 : Math.Round((Spent / BudgetLimit) * 100m, 1);
}
