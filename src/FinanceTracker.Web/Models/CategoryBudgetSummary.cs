namespace SelfRelianceFinanceTracker.Web.Models;

public class CategoryBudgetSummary
{
    public string CategoryName { get; set; } = string.Empty;
    public decimal Limit { get; set; }
    public decimal Spent { get; set; }

    public decimal Percentage =>
        Limit == 0 ? 0 : Math.Min(100, (Spent / Limit) * 100);
}