namespace SelfRelianceFinanceTracker.Web.Models;

public class AlertNotification
{
    public AlertSeverity Severity { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public string? ActionText { get; init; }
    public string? ActionHref { get; init; }
}
