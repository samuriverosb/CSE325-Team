using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SelfRelianceFinanceTracker.Web.Data;
using SelfRelianceFinanceTracker.Web.Models;

namespace SelfRelianceFinanceTracker.Web.Services;

public class CategoryService(ApplicationDbContext dbContext) : ICategoryService
{
    public async Task<IReadOnlyList<Category>> GetForUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await dbContext.Categories
                .Where(c => c.UserId == userId)
                .OrderBy(c => c.Name)
                .ToListAsync(cancellationToken);
        }
        catch (SqliteException)
        {
            return [];
        }
    }
}
