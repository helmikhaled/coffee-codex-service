using CoffeeCodex.Domain.Recipes;
using CoffeeCodex.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CoffeeCodex.Infrastructure.Persistence;

public sealed class CoffeeCodexDbContext(DbContextOptions<CoffeeCodexDbContext> options)
    : DbContext(options)
{
    public DbSet<Author> Authors => Set<Author>();

    public DbSet<Recipe> Recipes => Set<Recipe>();

    public DbSet<RecipeBrewSpecs> RecipeBrewSpecs => Set<RecipeBrewSpecs>();

    public DbSet<RecipeImage> RecipeImages => Set<RecipeImage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AuthorConfiguration());
        modelBuilder.ApplyConfiguration(new RecipeConfiguration());
        modelBuilder.ApplyConfiguration(new RecipeBrewSpecsConfiguration());
        modelBuilder.ApplyConfiguration(new RecipeImageConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}
