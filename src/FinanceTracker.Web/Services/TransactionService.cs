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

    public async Task CreateAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        dbContext.Transactions.Add(transaction);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var transaction = await dbContext.Transactions.FindAsync([id], cancellationToken);
        if (transaction is null) return;

        dbContext.Transactions.Remove(transaction);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.Transactions.FindAsync([transaction.Id], cancellationToken);
        if (existing is null) return;

        existing.Amount = transaction.Amount;
        existing.Type = transaction.Type;
        existing.CategoryId = transaction.CategoryId;
        existing.Date = transaction.Date;
        existing.Description = transaction.Description;

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
