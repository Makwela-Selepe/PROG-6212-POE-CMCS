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
        _filePath = Path.Combine(env.ContentRootPath, "App_Data", "claims.json");
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
        var list = await GetAllAsync();
        return list.FirstOrDefault(x => x.Id == id);
    }

    public async Task AddAsync(Claim claim)
    {
        var list = await GetAllAsync();
        list.Add(claim);
        Save(list);
    }

    public async Task UpdateAsync(Claim claim)
    {
        var list = await GetAllAsync();
        var i = list.FindIndex(x => x.Id == claim.Id);
        if (i >= 0)
        {
            list[i] = claim;
            Save(list);
        }
    }

    private void Save(List<Claim> list)
    {
        lock (_lock)
        {
            var text = JsonSerializer.Serialize(list, _json);
            File.WriteAllText(_filePath, text);
        }
    }
}
