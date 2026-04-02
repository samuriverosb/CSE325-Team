using System.ComponentModel.DataAnnotations;
using SelfRelianceFinanceTracker.Web.Data;

namespace SelfRelianceFinanceTracker.Web.Models;

public class SavingsGoal : IValidatableObject
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Goal name is required.")]
    [StringLength(120, MinimumLength = 2, ErrorMessage = "Goal name must be between 2 and 120 characters.")]
    public string GoalName { get; set; } = string.Empty;

    [Range(typeof(decimal), "1", "10000000",
        ConvertValueInInvariantCulture = true,
        ParseLimitsInInvariantCulture = true,
        ErrorMessage = "Target amount must be between 1 and 10,000,000.")]
    public decimal TargetAmount { get; set; }

    [Range(typeof(decimal), "0", "10000000",
        ConvertValueInInvariantCulture = true,
        ParseLimitsInInvariantCulture = true,
        ErrorMessage = "Current amount must be between 0 and 10,000,000.")]
    public decimal CurrentAmount { get; set; }

    [Required(ErrorMessage = "Deadline is required.")]
    public DateTime Deadline { get; set; } = DateTime.Now.Date.AddMonths(6);

    public string UserId { get; set; } = string.Empty;

    public ApplicationUser? User { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Deadline.Date < DateTime.Today)
        {
            yield return new ValidationResult(
                "Deadline cannot be in the past.",
                [nameof(Deadline)]);
        }

        if (CurrentAmount > TargetAmount)
        {
            yield return new ValidationResult(
                "Current amount cannot exceed the target amount.",
                [nameof(CurrentAmount), nameof(TargetAmount)]);
        }
    }
}
