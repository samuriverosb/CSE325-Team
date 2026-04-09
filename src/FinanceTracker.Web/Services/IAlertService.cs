using SelfRelianceFinanceTracker.Web.Models;

namespace SelfRelianceFinanceTracker.Web.Services;

public interface IAlertService
{
    Task<IReadOnlyList<AlertNotification>> GetAlertsForUserAsync(string userId, CancellationToken cancellationToken = default);
}
