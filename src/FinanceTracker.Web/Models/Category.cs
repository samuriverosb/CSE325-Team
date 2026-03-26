using System.ComponentModel.DataAnnotations;
using SelfRelianceFinanceTracker.Web.Data;

namespace SelfRelianceFinanceTracker.Web.Models;

public class Category
{
    public int Id { get; set; }

    [Required]
    [StringLength(80, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Range(0, 1_000_000)]
    public decimal MonthlyLimit { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public ApplicationUser? User { get; set; }

    public ICollection<Transaction> Transactions { get; set; } = [];
}
