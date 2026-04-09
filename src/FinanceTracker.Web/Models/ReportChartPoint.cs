namespace SelfRelianceFinanceTracker.Web.Models;

public class ReportChartPoint
{
    public string Label { get; init; } = string.Empty;
    public string Caption { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public decimal Percentage { get; init; }
    public string FillClass { get; init; } = "bg-primary";
}
