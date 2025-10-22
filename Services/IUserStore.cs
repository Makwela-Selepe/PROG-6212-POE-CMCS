using ContractMonthlyClaimSystem.Models;

namespace ContractMonthlyClaimSystem.Services;

public interface IUserStore
{
    Task<AppUser?> FindByEmailAsync(string email);
    Task<AppUser?> FindByEmailAndRoleAsync(string email, UserRole role);
    Task<bool> ValidatePasswordAsync(string email, string password);
    Task<AppUser?> CreateLecturerAsync(string name, string email, string password);
    Task SeedAsync();
}