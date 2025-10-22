using ContractMonthlyClaimSystem.Models;

namespace ContractMonthlyClaimSystem.Services;

public interface IClaimStore
{
    Task<List<Claim>> GetAllAsync();
    Task<Claim?> GetAsync(Guid id);
    Task AddAsync(Claim claim);
    Task UpdateAsync(Claim claim);
    Task<List<Claim>> GetByLecturerEmailAsync(string email);
}