namespace SelfRelianceFinanceTracker.Web.Models;

public class AlertNotification
{
    public string Title { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public string Area { get; init; } = string.Empty;
    public string SuggestedAction { get; init; } = string.Empty;
    public string SuggestedRoute { get; init; } = string.Empty;
    public AlertSeverity Severity { get; init; }

    public string BadgeClass =>
        Severity switch
        {
            AlertSeverity.Danger => "text-bg-danger",
            AlertSeverity.Warning => "text-bg-warning",
            _ => "text-bg-info"
        };
}
