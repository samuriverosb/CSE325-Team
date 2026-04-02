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

    public async Task CreateAsync(Category category, CancellationToken cancellationToken = default)
    {
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await dbContext.Categories.FindAsync([id], cancellationToken);
        if (category is null) return;

        dbContext.Categories.Remove(category);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Category category, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.Categories.FindAsync([category.Id], cancellationToken);
        if (existing is null) return;

        existing.Name = category.Name;
        existing.MonthlyLimit = category.MonthlyLimit;

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
