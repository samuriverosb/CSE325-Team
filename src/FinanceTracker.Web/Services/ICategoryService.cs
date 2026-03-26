using SelfRelianceFinanceTracker.Web.Models;

namespace SelfRelianceFinanceTracker.Web.Services;

public interface ICategoryService
{
    Task<IReadOnlyList<Category>> GetForUserAsync(string userId, CancellationToken cancellationToken = default);
}
