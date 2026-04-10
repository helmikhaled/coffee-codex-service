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

    [Fact]
    public async Task GetRecipesAsync_WhenCategoryFilterIsProvided_ReturnsOnlyMatchingCategory()
    {
        await using var dbContext = CreateDbContext();
        await RecipeListingTestData.SeedAsync(dbContext);
        var reader = new RecipeSummaryReader(dbContext);

        var response = await reader.GetRecipesAsync(new GetRecipesQuery(
            Page: 1,
            PageSize: 12,
            Category: "Modern"));

        Assert.Equal(1, response.TotalCount);
        Assert.Single(response.Items);
        Assert.Equal("matcha-cloud", response.Items[0].Slug);
    }

    [Fact]
    public async Task GetRecipesAsync_WhenTagFilterIsProvided_ReturnsMatchingRecipes()
    {
        await using var dbContext = CreateDbContext();
        await RecipeListingTestData.SeedAsync(dbContext);
        var reader = new RecipeSummaryReader(dbContext);

        var response = await reader.GetRecipesAsync(new GetRecipesQuery(
            Page: 1,
            PageSize: 12,
            Tags: ["citrus"]));

        Assert.Equal(1, response.TotalCount);
        Assert.Single(response.Items);
        Assert.Equal("espresso-tonic", response.Items[0].Slug);
    }

    [Fact]
    public async Task GetRecipesAsync_WhenMultipleTagsAreProvided_UsesOrMatching()
    {
        await using var dbContext = CreateDbContext();
        await RecipeListingTestData.SeedAsync(dbContext);
        var reader = new RecipeSummaryReader(dbContext);

        var response = await reader.GetRecipesAsync(new GetRecipesQuery(
            Page: 1,
            PageSize: 12,
            Tags: ["citrus", "iced"]));

        Assert.Equal(2, response.TotalCount);
        Assert.Collection(
            response.Items,
            item => Assert.Equal("espresso-tonic", item.Slug),
            item => Assert.Equal("iced-maple-latte", item.Slug));
    }

    [Fact]
    public async Task GetRecipesAsync_WhenCategoryAndTagFiltersAreCombined_AppliesAndSemantics()
    {
        await using var dbContext = CreateDbContext();
        await RecipeListingTestData.SeedAsync(dbContext);
        var reader = new RecipeSummaryReader(dbContext);

        var response = await reader.GetRecipesAsync(new GetRecipesQuery(
            Page: 1,
            PageSize: 12,
            Category: "Iced",
            Tags: ["citrus", "iced"]));

        Assert.Equal(1, response.TotalCount);
        Assert.Single(response.Items);
        Assert.Equal("iced-maple-latte", response.Items[0].Slug);
    }

    [Fact]
    public async Task GetRecipesAsync_WhenFiltersAreApplied_PreservesPaginationAndTotalCount()
    {
        await using var dbContext = CreateDbContext();
        await RecipeListingTestData.SeedAsync(dbContext);
        var reader = new RecipeSummaryReader(dbContext);

        var response = await reader.GetRecipesAsync(new GetRecipesQuery(
            Page: 1,
            PageSize: 2,
            Tags: ["citrus", "iced", "dessert"]));

        Assert.Equal(3, response.TotalCount);
        Assert.Equal(2, response.Items.Count);
        Assert.Collection(
            response.Items,
            item => Assert.Equal("espresso-tonic", item.Slug),
            item => Assert.Equal("iced-maple-latte", item.Slug));
    }

    [Fact]
    public async Task GetRecipesAsync_WhenNoFilteredMatchesExist_ReturnsEmptyItemsAndZeroTotalCount()
    {
        await using var dbContext = CreateDbContext();
        await RecipeListingTestData.SeedAsync(dbContext);
        var reader = new RecipeSummaryReader(dbContext);

        var response = await reader.GetRecipesAsync(new GetRecipesQuery(
            Page: 1,
            PageSize: 12,
            Tags: ["non-existent-tag"]));

        Assert.Equal(0, response.TotalCount);
        Assert.Empty(response.Items);
    }

    private static CoffeeCodexDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<CoffeeCodexDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new CoffeeCodexDbContext(options);
    }
}
