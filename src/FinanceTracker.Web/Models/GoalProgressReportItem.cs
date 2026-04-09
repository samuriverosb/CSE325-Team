namespace SelfRelianceFinanceTracker.Web.Models;

public class GoalProgressReportItem
{
    public string GoalName { get; init; } = string.Empty;
    public decimal CurrentAmount { get; init; }
    public decimal TargetAmount { get; init; }
    public DateTime Deadline { get; init; }

    public decimal ProgressPercentage =>
        TargetAmount <= 0 ? 0m : Math.Min(100m, (CurrentAmount / TargetAmount) * 100m);

    public int DaysRemaining => (Deadline.Date - DateTime.Today).Days;

    public string Status =>
        CurrentAmount >= TargetAmount ? "Completed" :
        DaysRemaining < 0 ? "Past due" :
        DaysRemaining <= 14 ? "Due soon" :
        "On track";
}
