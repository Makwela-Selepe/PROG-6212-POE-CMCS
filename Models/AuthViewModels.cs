using System.ComponentModel.DataAnnotations;

namespace ContractMonthlyClaimSystem.Models;

public class LoginViewModel
{
    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = "";

    [Required]
    public UserRole Role { get; set; } = UserRole.Lecturer; // Fixed: Changed from AppUser to UserRole
}

public class RegisterViewModel
{
    [Required]
    public string Name { get; set; } = "";

    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = "";
}

