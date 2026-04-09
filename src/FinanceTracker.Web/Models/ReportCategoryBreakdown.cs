namespace SelfRelianceFinanceTracker.Web.Models;

public class ReportCategoryBreakdown
{
    public string CategoryName { get; init; } = string.Empty;
    public decimal Limit { get; init; }
    public decimal Spent { get; init; }
    public decimal ExpenseSharePercentage { get; init; }

    public decimal Remaining => Math.Max(0m, Limit - Spent);

    public decimal UsagePercentage =>
        Limit <= 0 ? 0m : Math.Min(100m, (Spent / Limit) * 100m);

    public string Status =>
        Limit <= 0 ? "No limit" :
        Spent > Limit ? "Over budget" :
        UsagePercentage >= 90m ? "Near limit" :
        "On track";
}
