using CoffeeCodex.Application.Recipes.Queries.GetRecipes;
using CoffeeCodex.Infrastructure.Persistence;
using CoffeeCodex.Infrastructure.Recipes;
using Microsoft.EntityFrameworkCore;

namespace CoffeeCodex.RecipeListing.Tests.Infrastructure;

public sealed class RecipeSummaryReaderTest
{
    [Fact]
    public async Task GetRecipesAsync_ReturnsPagedResultsWithTotalCount()
    {
        await using var dbContext = CreateDbContext();
        await RecipeListingTestData.SeedAsync(dbContext);
        var reader = new RecipeSummaryReader(dbContext);

        var response = await reader.GetRecipesAsync(new GetRecipesQuery(Page: 1, PageSize: 2));

        Assert.Equal(4, response.TotalCount);
        Assert.Equal(2, response.Items.Count);
        Assert.Equal(1, response.Page);
        Assert.Equal(2, response.PageSize);
    }

    [Fact]
    public async Task GetRecipesAsync_OrdersByDisplayOrderThenId()
    {
        await using var dbContext = CreateDbContext();
        await RecipeListingTestData.SeedAsync(dbContext);
        var reader = new RecipeSummaryReader(dbContext);

        var response = await reader.GetRecipesAsync(new GetRecipesQuery(Page: 1, PageSize: 4));

        Assert.Collection(
            response.Items,
            item => Assert.Equal("espresso-tonic", item.Slug),
            item => Assert.Equal("iced-maple-latte", item.Slug),
            item => Assert.Equal("matcha-cloud", item.Slug),
            item => Assert.Equal("affogato-float", item.Slug));
    }

    [Fact]
    public async Task GetRecipesAsync_SelectsThumbnailFromPositionOne()
    {
        await using var dbContext = CreateDbContext();
        await RecipeListingTestData.SeedAsync(dbContext);
        var reader = new RecipeSummaryReader(dbContext);

        var response = await reader.GetRecipesAsync(new GetRecipesQuery(Page: 1, PageSize: 4));

        Assert.Equal(
            "https://cdn.example.com/recipes/espresso-tonic-thumb.jpg",
            response.Items[0].ThumbnailUrl);
    }

    [Fact]
    public async Task GetRecipesAsync_ReturnsNullThumbnailWhenRecipeHasNoImage()
    {
        await using var dbContext = CreateDbContext();
        await RecipeListingTestData.SeedAsync(dbContext);
        var reader = new RecipeSummaryReader(dbContext);

        var response = await reader.GetRecipesAsync(new GetRecipesQuery(Page: 1, PageSize: 4));

        Assert.Null(response.Items[2].ThumbnailUrl);
    }

    [Fact]
    public async Task GetRecipesAsync_ReturnsEmptySetWhenNoRecipesExist()
    {
        await using var dbContext = CreateDbContext();
        var reader = new RecipeSummaryReader(dbContext);

        var response = await reader.GetRecipesAsync(new GetRecipesQuery(Page: 1, PageSize: 12));

        Assert.Empty(response.Items);
        Assert.Equal(0, response.TotalCount);
    }

    private static CoffeeCodexDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<CoffeeCodexDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new CoffeeCodexDbContext(options);
    }
}
