using CoffeeCodex.Application.Recipes.Commands.RecordRecipeView;
using CoffeeCodex.Infrastructure.Persistence;
using CoffeeCodex.Infrastructure.Recipes;
using Microsoft.EntityFrameworkCore;

namespace CoffeeCodex.RecipeListing.Tests.Infrastructure;

public sealed class RecipeViewRecorderTest
{
    [Fact]
    public async Task RecordViewAsync_WhenRecipeExists_IncrementsBrewCountByOne()
    {
        await using var dbContext = CreateDbContext();
        await RecipeListingTestData.SeedAsync(dbContext);
        var recorder = new RecipeViewRecorder(dbContext);

        var wasRecorded = await recorder.RecordViewAsync(
            new RecordRecipeViewCommand(RecipeListingTestData.EspressoTonicId));

        var updatedRecipe = await dbContext.Recipes.SingleAsync(
            recipe => recipe.Id == RecipeListingTestData.EspressoTonicId);

        Assert.True(wasRecorded);
        Assert.Equal(125, updatedRecipe.BrewCount);
    }

    [Fact]
    public async Task RecordViewAsync_WhenRecipeDoesNotExist_ReturnsFalse()
    {
        await using var dbContext = CreateDbContext();
        await RecipeListingTestData.SeedAsync(dbContext);
        var recorder = new RecipeViewRecorder(dbContext);

        var wasRecorded = await recorder.RecordViewAsync(new RecordRecipeViewCommand(Guid.NewGuid()));

        Assert.False(wasRecorded);
    }

    [Fact]
    public async Task RecordViewAsync_WhenCalledTwice_IncrementsBrewCountTwice()
    {
        await using var dbContext = CreateDbContext();
        await RecipeListingTestData.SeedAsync(dbContext);
        var recorder = new RecipeViewRecorder(dbContext);

        var firstResult = await recorder.RecordViewAsync(
            new RecordRecipeViewCommand(RecipeListingTestData.EspressoTonicId));
        var secondResult = await recorder.RecordViewAsync(
            new RecordRecipeViewCommand(RecipeListingTestData.EspressoTonicId));

        var updatedRecipe = await dbContext.Recipes.SingleAsync(
            recipe => recipe.Id == RecipeListingTestData.EspressoTonicId);

        Assert.True(firstResult);
        Assert.True(secondResult);
        Assert.Equal(126, updatedRecipe.BrewCount);
    }

    private static CoffeeCodexDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<CoffeeCodexDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new CoffeeCodexDbContext(options);
    }
}
