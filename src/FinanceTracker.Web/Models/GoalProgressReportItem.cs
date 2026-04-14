namespace SelfRelianceFinanceTracker.Web.Models;

public class GoalProgressReportItem
{
    public string GoalName { get; init; } = string.Empty;
    public decimal CurrentAmount { get; init; }
    public decimal TargetAmount { get; init; }
    public DateTime Deadline { get; init; }

    public decimal RemainingAmount => Math.Max(0, TargetAmount - CurrentAmount);

    public decimal ProgressPercentage =>
        TargetAmount <= 0 ? 0 : Math.Round((CurrentAmount / TargetAmount) * 100m, 1);

    public int DaysRemaining => (Deadline.Date - DateTime.Today).Days;
}
