using ContractMonthlyClaimSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ContractMonthlyClaimSystem.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Claim> Claims => Set<Claim>();
    public DbSet<UploadMeta> Uploads => Set<UploadMeta>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Unique email per user
        modelBuilder.Entity<AppUser>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Claim -> Uploads relation (one claim, many uploads)
        modelBuilder.Entity<Claim>()
            .HasMany(c => c.Uploads)
            .WithOne()
            .HasForeignKey("ClaimId")
            .OnDelete(DeleteBehavior.Cascade);

        // Money fields: decimal(18,2)
        modelBuilder.Entity<Claim>()
            .Property(c => c.HourlyRate)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<AppUser>()
            .Property(u => u.HourlyRate)
            .HasColumnType("decimal(18,2)");
    }
}
