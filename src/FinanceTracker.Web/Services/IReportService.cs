using SelfRelianceFinanceTracker.Web.Models;

namespace SelfRelianceFinanceTracker.Web.Services;

public interface IReportService
{
    Task<MonthlyReport> GetCurrentMonthReportAsync(string userId, CancellationToken cancellationToken = default);
}
