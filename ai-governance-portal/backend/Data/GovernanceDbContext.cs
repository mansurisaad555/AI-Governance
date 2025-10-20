using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public class GovernanceDbContext : DbContext
{
    public GovernanceDbContext(DbContextOptions<GovernanceDbContext> options)
        : base(options)
    {
    }

    public DbSet<UsageEntry> UsageEntries { get; set; } = null!;
    public DbSet<ModelCard> ModelCards { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UsageEntry>()
            .HasOne(e => e.ModelCard)
            .WithOne(c => c.UsageEntry)
            .HasForeignKey<ModelCard>(c => c.UsageEntryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
