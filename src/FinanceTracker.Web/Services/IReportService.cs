using SelfRelianceFinanceTracker.Web.Models;

namespace SelfRelianceFinanceTracker.Web.Services;

public interface IReportService
{
    Task<MonthlyReport> GetMonthlyReportAsync(string userId, CancellationToken cancellationToken = default);
}
