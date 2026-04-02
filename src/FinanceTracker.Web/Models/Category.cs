using System.ComponentModel.DataAnnotations;
using SelfRelianceFinanceTracker.Web.Data;

namespace SelfRelianceFinanceTracker.Web.Models;

public class Category
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Category name is required.")]
    [StringLength(80, MinimumLength = 2, ErrorMessage = "Category name must be between 2 and 80 characters.")]
    public string Name { get; set; } = string.Empty;

    [Range(typeof(decimal), "0", "1000000",
        ConvertValueInInvariantCulture = true,
        ParseLimitsInInvariantCulture = true,
        ErrorMessage = "Monthly limit must be between 0 and 1,000,000.")]
    public decimal MonthlyLimit { get; set; }

    public string UserId { get; set; } = string.Empty;

    public ApplicationUser? User { get; set; }

    public ICollection<Transaction> Transactions { get; set; } = [];
}
