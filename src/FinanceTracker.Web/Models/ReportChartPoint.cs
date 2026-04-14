namespace SelfRelianceFinanceTracker.Web.Models;

public class ReportChartPoint
{
    public string Label { get; init; } = string.Empty;
    public decimal Value { get; init; }
    public decimal Percentage { get; init; }
}
