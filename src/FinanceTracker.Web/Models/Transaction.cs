using System.ComponentModel.DataAnnotations;
using SelfRelianceFinanceTracker.Web.Data;

namespace SelfRelianceFinanceTracker.Web.Models;

public class Transaction
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Category is required")]
    public int CategoryId { get; set; }

    [Range(0.01, 1000000000)]
    public decimal Amount { get; set; }

    [Required]
    public TransactionType Type { get; set; } = TransactionType.Expense;

    [Required]
    public DateTime Date { get; set; } = DateTime.UtcNow.Date;

    [StringLength(250)]
    public string? Description { get; set; }

    public ApplicationUser? User { get; set; }
    public Category? Category { get; set; }
}
