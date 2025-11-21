using System.Text.Json;
using ContractMonthlyClaimSystem.Models;

namespace ContractMonthlyClaimSystem.Services;

public class JsonUserStore : IUserStore
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _json = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    private readonly object _lock = new();

    public JsonUserStore(IWebHostEnvironment env)
    {
        var dataPath = Path.Combine(env.ContentRootPath, "App_Data");
        Directory.CreateDirectory(dataPath);
        _filePath = Path.Combine(dataPath, "users.json");

        if (!File.Exists(_filePath))
        {
            File.WriteAllText(_filePath, "[]");
            SeedAsync().Wait(); // Seed initial users
        }
    }

    public Task<List<AppUser>> GetAllAsync()
    {
        lock (_lock)
        {
            var text = File.ReadAllText(_filePath);
            return Task.FromResult(JsonSerializer.Deserialize<List<AppUser>>(text, _json) ?? new());
        }
    }

    public async Task<AppUser?> FindByEmailAsync(string email)
    {
        var users = await GetAllAsync();
        return users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<AppUser?> FindByEmailAndRoleAsync(string email, UserRole role)
    {
        var users = await GetAllAsync();
        return users.FirstOrDefault(u =>
            u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
            u.Role == role);
    }

    public async Task<bool> ValidatePasswordAsync(string email, string password)
    {
        var user = await FindByEmailAsync(email);
        if (user == null) return false;

        // Simple demo hashing – fine for this assignment
        return user.PasswordHash == HashPassword(password);
    }

    public async Task<AppUser?> CreateLecturerAsync(string name, string email, string password)
    {
        var existing = await FindByEmailAsync(email);
        if (existing != null) return null;

        var newUser = new AppUser
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            PasswordHash = HashPassword(password),
            Role = UserRole.Lecturer
        };

        var users = await GetAllAsync();
        users.Add(newUser);
        await SaveAsync(users);

        return newUser;
    }

    public async Task SeedAsync()
    {
        var users = await GetAllAsync();

        // Remove old defaults if they exist
        users.RemoveAll(u =>
            u.Email.Equals("sm@gmail.com", StringComparison.OrdinalIgnoreCase) ||
            u.Email.Equals("buhle@example.com", StringComparison.OrdinalIgnoreCase) ||
            u.Email.Equals("mike@example.com", StringComparison.OrdinalIgnoreCase) ||
            u.Email.Equals("hr@example.com", StringComparison.OrdinalIgnoreCase));

        // Default users (now includes HR)
        var defaultUsers = new[]
        {
    new { Name = "Selepe",   Email = "sm@gmail.com",        Password = "Sammyru12@", Role = UserRole.Lecturer,   HourlyRate = 350m },
    new { Name = "Buhle",    Email = "buhle@example.com",   Password = "Sammyru12@", Role = UserRole.Coordinator, HourlyRate = 0m   },
    new { Name = "Mike",     Email = "mike@example.com",    Password = "Sammyru12@", Role = UserRole.Manager,     HourlyRate = 0m   },
    new { Name = "HR Admin", Email = "hr@example.com",      Password = "Sammyru12@", Role = UserRole.HR,         HourlyRate = 0m   }
};


        foreach (var user in defaultUsers)
        {
            if (!users.Any(u => u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)))
            {
                users.Add(new AppUser
                {
                    Id = Guid.NewGuid(),
                    Name = user.Name,
                    Email = user.Email,
                    PasswordHash = HashPassword(user.Password),
                    Role = user.Role
                });
            }
        }

        await SaveAsync(users);
    }

    public Task SaveAsync(List<AppUser> users)
    {
        lock (_lock)
        {
            var text = JsonSerializer.Serialize(users, _json);
            File.WriteAllText(_filePath, text);
        }

        return Task.CompletedTask;
    }

    private string HashPassword(string password)
    {
        // Simple SHA256 hash – good enough for the assignment
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(password);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
