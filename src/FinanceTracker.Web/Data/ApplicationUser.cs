using Microsoft.AspNetCore.Identity;
using SelfRelianceFinanceTracker.Web.Models;

namespace SelfRelianceFinanceTracker.Web.Data;

public class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }

    public ICollection<Category> Categories { get; set; } = [];
    public ICollection<Transaction> Transactions { get; set; } = [];
    public ICollection<SavingsGoal> SavingsGoals { get; set; } = [];
}

