using System.ComponentModel.DataAnnotations;

namespace ContractMonthlyClaimSystem.Models;

public class Claim
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, StringLength(80)]
    public string LecturerName { get; set; } = "";

    [Required, EmailAddress, StringLength(100)]
    public string Email { get; set; } = "";

    [Range(1, 250)]
    public int HoursWorked { get; set; }

    [Range(50, 2000)]
    public decimal HourlyRate { get; set; }

    [StringLength(250)]
    public string? Notes { get; set; }

    public decimal Total => HoursWorked * HourlyRate;

    // Status flow
    public ClaimStatus Status { get; set; } = ClaimStatus.Pending;
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    // Uploaded files (optional)
    public List<UploadMeta> Uploads { get; set; } = new();

    // Removed duplicate properties - only keeping LecturerEmail
    public string LecturerEmail { get; set; } = "";
}