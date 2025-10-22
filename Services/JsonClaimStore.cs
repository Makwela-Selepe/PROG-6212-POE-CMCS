using System.Text.Json;
using ContractMonthlyClaimSystem.Models;

namespace ContractMonthlyClaimSystem.Services;

public class JsonClaimStore : IClaimStore
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _json = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    private readonly object _lock = new();

    public JsonClaimStore(IWebHostEnvironment env)
    {
        var dataPath = Path.Combine(env.ContentRootPath, "App_Data");
        Directory.CreateDirectory(dataPath);
        _filePath = Path.Combine(dataPath, "claims.json");

        if (!File.Exists(_filePath))
        {
            File.WriteAllText(_filePath, "[]");
        }
    }

    public Task<List<Claim>> GetAllAsync()
    {
        lock (_lock)
        {
            var text = File.ReadAllText(_filePath);
            return Task.FromResult(JsonSerializer.Deserialize<List<Claim>>(text, _json) ?? new());
        }
    }

    public async Task<Claim?> GetAsync(Guid id)
    {
        var claims = await GetAllAsync();
        return claims.FirstOrDefault(x => x.Id == id);
    }

    public async Task AddAsync(Claim claim)
    {
        var claims = await GetAllAsync();
        claims.Add(claim);
        await SaveAsync(claims);
    }

    public async Task UpdateAsync(Claim claim)
    {
        var claims = await GetAllAsync();
        var index = claims.FindIndex(x => x.Id == claim.Id);
        if (index >= 0)
        {
            claims[index] = claim;
            await SaveAsync(claims);
        }
    }

    public async Task<List<Claim>> GetByLecturerEmailAsync(string email)
    {
        var claims = await GetAllAsync();
        return claims.Where(c => c.LecturerEmail.Equals(email, StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(c => c.CreatedUtc)
                    .ToList();
    }

    private async Task SaveAsync(List<Claim> claims)
    {
        lock (_lock)
        {
            var text = JsonSerializer.Serialize(claims, _json);
            File.WriteAllText(_filePath, text);
        }
    }
}