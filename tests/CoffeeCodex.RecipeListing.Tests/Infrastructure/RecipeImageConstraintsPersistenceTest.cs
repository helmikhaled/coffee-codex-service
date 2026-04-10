using CoffeeCodex.Domain.Recipes;
using CoffeeCodex.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CoffeeCodex.RecipeListing.Tests.Infrastructure;

public sealed class RecipeImageConstraintsPersistenceTest
{
    [Fact]
    public async Task SaveChangesAsync_WhenImagePositionIsLessThanOne_ThrowsDbUpdateException()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<CoffeeCodexDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var dbContext = new CoffeeCodexDbContext(options);
        await dbContext.Database.EnsureCreatedAsync();

        var recipeId = await SeedRecipeAsync(dbContext);

        dbContext.RecipeImages.Add(new RecipeImage
        {
            Id = Guid.NewGuid(),
            RecipeId = recipeId,
            BlobUrl = "https://cdn.example.com/recipes/invalid-position.jpg",
            Caption = null,
            Position = 0,
            CreatedAt = DateTime.UtcNow,
        });

        await Assert.ThrowsAsync<DbUpdateException>(() => dbContext.SaveChangesAsync());
    }

    [Fact]
    public async Task SaveChangesAsync_WhenRecipeHasDuplicateImagePositions_ThrowsDbUpdateException()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<CoffeeCodexDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var dbContext = new CoffeeCodexDbContext(options);
        await dbContext.Database.EnsureCreatedAsync();

        var recipeId = await SeedRecipeAsync(dbContext);

        dbContext.RecipeImages.Add(new RecipeImage
        {
            Id = Guid.NewGuid(),
            RecipeId = recipeId,
            BlobUrl = "https://cdn.example.com/recipes/primary.jpg",
            Caption = "Primary",
            Position = 1,
            CreatedAt = DateTime.UtcNow,
        });
        await dbContext.SaveChangesAsync();

        dbContext.RecipeImages.Add(new RecipeImage
        {
            Id = Guid.NewGuid(),
            RecipeId = recipeId,
            BlobUrl = "https://cdn.example.com/recipes/duplicate.jpg",
            Caption = "Duplicate position",
            Position = 1,
            CreatedAt = DateTime.UtcNow,
        });

        await Assert.ThrowsAsync<DbUpdateException>(() => dbContext.SaveChangesAsync());
    }

    private static async Task<Guid> SeedRecipeAsync(CoffeeCodexDbContext dbContext)
    {
        var author = new Author
        {
            Id = Guid.NewGuid(),
            Name = "Constraint Test Author",
            AvatarUrl = null,
        };

        var recipe = new Recipe
        {
            Id = Guid.NewGuid(),
            Title = "Constraint Test Recipe",
            Description = "Recipe used for recipe image constraint tests.",
            Category = RecipeCategory.Modern,
            Slug = $"constraint-test-recipe-{Guid.NewGuid():N}",
            AuthorId = author.Id,
            BrewCount = 0,
            DisplayOrder = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        dbContext.Authors.Add(author);
        dbContext.Recipes.Add(recipe);
        await dbContext.SaveChangesAsync();

        return recipe.Id;
    }
}
