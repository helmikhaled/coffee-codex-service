using CoffeeCodex.Application.Recipes.Queries.GetRecipeDetail;
using CoffeeCodex.Infrastructure.Persistence;
using CoffeeCodex.Infrastructure.Recipes;
using Microsoft.EntityFrameworkCore;

namespace CoffeeCodex.RecipeListing.Tests.Infrastructure;

public sealed class RecipeDetailReaderTest
{
    [Fact]
    public async Task GetRecipeDetailAsync_ReturnsCompleteDetailPayload()
    {
        await using var dbContext = CreateDbContext();
        await RecipeListingTestData.SeedAsync(dbContext);
        var reader = new RecipeDetailReader(dbContext);

        var result = await reader.GetRecipeDetailAsync(
            new GetRecipeDetailQuery(RecipeListingTestData.EspressoTonicId));

        Assert.NotNull(result);
        Assert.Equal(RecipeListingTestData.EspressoTonicId, result.Id);
        Assert.Equal("espresso-tonic", result.Slug);
        Assert.Equal("Espresso Tonic", result.Title);
        Assert.Equal("Bright espresso with tonic water and citrus.", result.Description);
        Assert.Equal(124, result.BrewCount);
        Assert.Equal("Coffee Codex", result.Author.Name);
        Assert.NotNull(result.BrewSpecs);
        Assert.Equal(18m, result.BrewSpecs.CoffeeDoseInGrams);
        Assert.Equal(36m, result.BrewSpecs.CoffeeYieldInGrams);
        Assert.Equal(250, result.BrewSpecs.CupSizeInMl);
        Assert.NotEmpty(result.Ingredients);
        Assert.NotEmpty(result.Steps);
        Assert.NotEmpty(result.Images);
        Assert.NotEmpty(result.Tags);
    }

    [Fact]
    public async Task GetRecipeDetailAsync_ReturnsOrderedCollectionsAndTags()
    {
        await using var dbContext = CreateDbContext();
        await RecipeListingTestData.SeedAsync(dbContext);
        var reader = new RecipeDetailReader(dbContext);

        var result = await reader.GetRecipeDetailAsync(
            new GetRecipeDetailQuery(RecipeListingTestData.EspressoTonicId));

        Assert.NotNull(result);
        Assert.Equal([1, 2], result.Ingredients.Select(ingredient => ingredient.Position).ToArray());
        Assert.Equal([1, 2], result.Steps.Select(step => step.StepNumber).ToArray());
        Assert.Equal([1, 2], result.Images.Select(image => image.Position).ToArray());
        Assert.Equal(["citrus", "sparkling"], result.Tags);
    }

    [Fact]
    public async Task GetRecipeDetailAsync_ForNonEspressoRecipe_KeepsNullableCoffeeFields()
    {
        await using var dbContext = CreateDbContext();
        await RecipeListingTestData.SeedAsync(dbContext);
        var reader = new RecipeDetailReader(dbContext);

        var result = await reader.GetRecipeDetailAsync(
            new GetRecipeDetailQuery(RecipeListingTestData.MatchaCloudId));

        Assert.NotNull(result);
        Assert.NotNull(result.BrewSpecs);
        Assert.Null(result.BrewSpecs.CoffeeDoseInGrams);
        Assert.Null(result.BrewSpecs.CoffeeYieldInGrams);
    }

    [Fact]
    public async Task GetRecipeDetailAsync_WhenRecipeDoesNotExist_ReturnsNull()
    {
        await using var dbContext = CreateDbContext();
        await RecipeListingTestData.SeedAsync(dbContext);
        var reader = new RecipeDetailReader(dbContext);

        var result = await reader.GetRecipeDetailAsync(new GetRecipeDetailQuery(Guid.NewGuid()));

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
