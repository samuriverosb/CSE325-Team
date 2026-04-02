using System.ComponentModel.DataAnnotations;
using SelfRelianceFinanceTracker.Web.Data;

namespace SelfRelianceFinanceTracker.Web.Models;

public class SavingsGoal
{
    public int Id { get; set; }

    [Required]
    [StringLength(120, MinimumLength = 2)]
    public string GoalName { get; set; } = string.Empty;

    [Range(1, 10000000)]
    public decimal TargetAmount { get; set; }

    [Range(1, 10000000)]
    public decimal CurrentAmount { get; set; }

    [Required]
    public DateTime Deadline { get; set; } = DateTime.Now.Date.AddMonths(6);

    public string UserId { get; set; } = string.Empty;

    public ApplicationUser? User { get; set; }
}
