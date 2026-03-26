using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SelfRelianceFinanceTracker.Web.Data;
using SelfRelianceFinanceTracker.Web.Models;

namespace SelfRelianceFinanceTracker.Web.Services;

public class TransactionService(ApplicationDbContext dbContext) : ITransactionService
{
    public async Task<IReadOnlyList<Transaction>> GetRecentForUserAsync(string userId, int take = 10, CancellationToken cancellationToken = default)
    {
        try
        {
            return await dbContext.Transactions
                .AsNoTracking()
                .Include(t => t.Category)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.Date)
                .ThenByDescending(t => t.Id)
                .Take(Math.Clamp(take, 1, 50))
                .ToListAsync(cancellationToken);
        }
        catch (SqliteException)
        {
            return [];
        }
    }
}
