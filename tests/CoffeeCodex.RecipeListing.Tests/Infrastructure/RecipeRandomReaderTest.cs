using CoffeeCodex.Application.Recipes.Queries.GetRandomRecipe;
using CoffeeCodex.Infrastructure.Persistence;
using CoffeeCodex.Infrastructure.Recipes;
using Microsoft.EntityFrameworkCore;

namespace CoffeeCodex.RecipeListing.Tests.Infrastructure;

public sealed class RecipeRandomReaderTest
{
    [Fact]
    public async Task GetRandomRecipeAsync_WhenRecipesExist_ReturnsSeededRecipeId()
    {
        await using var dbContext = CreateDbContext();
        await RecipeListingTestData.SeedAsync(dbContext);
        var reader = new RecipeRandomReader(dbContext);

        var result = await reader.GetRandomRecipeAsync(new GetRandomRecipeQuery());

        Assert.NotNull(result);
        Assert.Contains(
            result.Id,
            new[]
            {
                RecipeListingTestData.EspressoTonicId,
                RecipeListingTestData.IcedMapleLatteId,
                RecipeListingTestData.MatchaCloudId,
                RecipeListingTestData.AffogatoFloatId,
            });
    }

    [Fact]
    public async Task GetRandomRecipeAsync_WhenNoRecipesExist_ReturnsNull()
    {
        await using var dbContext = CreateDbContext();
        var reader = new RecipeRandomReader(dbContext);

        var result = await reader.GetRandomRecipeAsync(new GetRandomRecipeQuery());

        Assert.Null(result);
    }

    private static CoffeeCodexDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<CoffeeCodexDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new CoffeeCodexDbContext(options);
    }
}
