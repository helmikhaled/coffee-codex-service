using Microsoft.EntityFrameworkCore;

namespace CoffeeCodex.Infrastructure.Persistence;

public sealed class CoffeeCodexDbContext(DbContextOptions<CoffeeCodexDbContext> options)
    : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
