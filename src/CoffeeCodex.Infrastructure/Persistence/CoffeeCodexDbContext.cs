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

    public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();

    public DbSet<RecipeStep> RecipeSteps => Set<RecipeStep>();

    public DbSet<Tag> Tags => Set<Tag>();

    public DbSet<RecipeTag> RecipeTags => Set<RecipeTag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("pg_trgm");

        modelBuilder.ApplyConfiguration(new AuthorConfiguration());
        modelBuilder.ApplyConfiguration(new RecipeConfiguration());
        modelBuilder.ApplyConfiguration(new RecipeBrewSpecsConfiguration());
        modelBuilder.ApplyConfiguration(new RecipeImageConfiguration());
        modelBuilder.ApplyConfiguration(new RecipeIngredientConfiguration());
        modelBuilder.ApplyConfiguration(new RecipeStepConfiguration());
        modelBuilder.ApplyConfiguration(new TagConfiguration());
        modelBuilder.ApplyConfiguration(new RecipeTagConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}
