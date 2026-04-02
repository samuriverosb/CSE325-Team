using System.ComponentModel.DataAnnotations;
using SelfRelianceFinanceTracker.Web.Data;

namespace SelfRelianceFinanceTracker.Web.Models;

public class Transaction
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "A category is required.")]
    public int CategoryId { get; set; }

    [Range(typeof(decimal), "0.01", "1000000000",
        ConvertValueInInvariantCulture = true,
        ParseLimitsInInvariantCulture = true,
        ErrorMessage = "Amount must be between 0.01 and 1,000,000,000.")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Transaction type is required.")]
    public TransactionType Type { get; set; } = TransactionType.Expense;

    [Required(ErrorMessage = "Transaction date is required.")]
    public DateTime Date { get; set; } = DateTime.UtcNow.Date;

    [StringLength(250, ErrorMessage = "Description cannot exceed 250 characters.")]
    public string? Description { get; set; }

    public ApplicationUser? User { get; set; }
    public Category? Category { get; set; }
}
