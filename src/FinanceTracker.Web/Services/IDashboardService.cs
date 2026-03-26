using SelfRelianceFinanceTracker.Web.Models;

namespace SelfRelianceFinanceTracker.Web.Services;

public interface IDashboardService
{
    Task<DashboardSummary> GetCurrentMonthSummaryAsync(string userId, CancellationToken cancellationToken = default);
}
