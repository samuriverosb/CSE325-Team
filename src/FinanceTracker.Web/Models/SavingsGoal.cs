using System.ComponentModel.DataAnnotations;
using SelfRelianceFinanceTracker.Web.Data;

namespace SelfRelianceFinanceTracker.Web.Models;

public class SavingsGoal
{
    public int Id { get; set; }

    [Required]
    [StringLength(120, MinimumLength = 2)]
    public string GoalName { get; set; } = string.Empty;

    [Range(typeof(decimal), "1", "10000000")]
    public decimal TargetAmount { get; set; }

    [Range(typeof(decimal), "0", "10000000")]
    public decimal CurrentAmount { get; set; }

    [Required]
    public DateTime Deadline { get; set; } = DateTime.UtcNow.Date.AddMonths(6);

    [Required]
    public string UserId { get; set; } = string.Empty;

    public ApplicationUser? User { get; set; }
}
