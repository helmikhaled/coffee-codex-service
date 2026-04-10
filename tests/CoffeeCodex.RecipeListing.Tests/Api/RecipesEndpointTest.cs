using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CoffeeCodex.Application.Recipes.Queries.GetRecipeDetail;
using CoffeeCodex.Application.Recipes.Queries.GetRecipes;
using Microsoft.AspNetCore.Hosting;
using CoffeeCodex.Infrastructure.Persistence;
using CoffeeCodex.Shared.Pagination;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoffeeCodex.RecipeListing.Tests.Api;

public sealed class RecipesEndpointTest : IClassFixture<RecipeListingApiFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
    };

    private readonly HttpClient _httpClient;

    public RecipesEndpointTest(RecipeListingApiFactory factory)
    {
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task GetRecipes_ReturnsOk()
    {
        using var response = await _httpClient.GetAsync("/recipes?page=1&pageSize=2");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetRecipes_ReturnsPagedPayload()
    {
        using var response = await _httpClient.GetAsync("/recipes?page=1&pageSize=2");
        var payload = await response.Content.ReadFromJsonAsync<PagedResponse<RecipeSummaryDto>>(JsonOptions);

        Assert.NotNull(payload);
        Assert.Equal(4, payload.TotalCount);
        Assert.Equal(2, payload.Items.Count);
    }

    [Fact]
    public async Task GetRecipes_WhenPageIsInvalid_ReturnsBadRequest()
    {
        using var response = await _httpClient.GetAsync("/recipes?page=0&pageSize=2");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetRecipes_WhenFilteredByCategory_ReturnsMatchingRecipes()
    {
        using var response = await _httpClient.GetAsync("/recipes?category=Modern&page=1&pageSize=12");
        var payload = await response.Content.ReadFromJsonAsync<PagedResponse<RecipeSummaryDto>>(JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.Equal(1, payload.TotalCount);
        Assert.Single(payload.Items);
        Assert.Equal("matcha-cloud", payload.Items[0].Slug);
    }

    [Fact]
    public async Task GetRecipes_WhenCategoryCasingIsInvalid_ReturnsBadRequest()
    {
        using var response = await _httpClient.GetAsync("/recipes?category=modern&page=1&pageSize=12");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetRecipes_WhenFilteredByTag_ReturnsMatchingRecipes()
    {
        using var response = await _httpClient.GetAsync("/recipes?tag=matcha&page=1&pageSize=12");
        var payload = await response.Content.ReadFromJsonAsync<PagedResponse<RecipeSummaryDto>>(JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.Equal(1, payload.TotalCount);
        Assert.Single(payload.Items);
        Assert.Equal("matcha-cloud", payload.Items[0].Slug);
    }

    [Fact]
    public async Task GetRecipes_WhenFilteredByMultipleTags_UsesOrSemantics()
    {
        using var response = await _httpClient.GetAsync("/recipes?tag=matcha&tag=iced&page=1&pageSize=12");
        var payload = await response.Content.ReadFromJsonAsync<PagedResponse<RecipeSummaryDto>>(JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.Equal(2, payload.TotalCount);
        Assert.Collection(
            payload.Items,
            item => Assert.Equal("iced-maple-latte", item.Slug),
            item => Assert.Equal("matcha-cloud", item.Slug));
    }

    [Fact]
    public async Task GetRecipes_WhenCategoryAndTagAreCombined_ReturnsMatchingRecipes()
    {
        using var response = await _httpClient.GetAsync("/recipes?category=Modern&tag=matcha&page=1&pageSize=12");
        var payload = await response.Content.ReadFromJsonAsync<PagedResponse<RecipeSummaryDto>>(JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.Equal(1, payload.TotalCount);
        Assert.Single(payload.Items);
        Assert.Equal("matcha-cloud", payload.Items[0].Slug);
    }

    [Fact]
    public async Task GetRecipeById_WhenRecipeExists_ReturnsOk()
    {
        using var response = await _httpClient.GetAsync($"/recipes/{RecipeListingTestData.EspressoTonicId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetRecipeById_ReturnsDetailPayload()
    {
        using var response = await _httpClient.GetAsync($"/recipes/{RecipeListingTestData.EspressoTonicId}");
        var payload = await response.Content.ReadFromJsonAsync<RecipeDetailDto>(JsonOptions);

        Assert.NotNull(payload);
        Assert.Equal(RecipeListingTestData.EspressoTonicId, payload.Id);
        Assert.Equal("espresso-tonic", payload.Slug);
        Assert.NotNull(payload.Author);
        Assert.NotEmpty(payload.Ingredients);
        Assert.NotEmpty(payload.Steps);
        Assert.NotEmpty(payload.Images);
        Assert.NotEmpty(payload.Tags);
    }

    [Fact]
    public async Task GetRecipeById_ImagesAreReturnedInPositionOrder()
    {
        using var response = await _httpClient.GetAsync($"/recipes/{RecipeListingTestData.EspressoTonicId}");
        var payload = await response.Content.ReadFromJsonAsync<RecipeDetailDto>(JsonOptions);

        Assert.NotNull(payload);
        Assert.Equal([1, 2], payload.Images.Select(image => image.Position).ToArray());
    }

    [Fact]
    public async Task GetRecipeById_ImagePayloadIncludesUrlCaptionAndPosition()
    {
        using var response = await _httpClient.GetAsync($"/recipes/{RecipeListingTestData.EspressoTonicId}");
        var payload = await response.Content.ReadFromJsonAsync<RecipeDetailDto>(JsonOptions);

        Assert.NotNull(payload);
        Assert.NotEmpty(payload.Images);
        Assert.All(payload.Images, image =>
        {
            Assert.False(string.IsNullOrWhiteSpace(image.Url));
            Assert.True(image.Position >= 1);
        });
    }

    [Fact]
    public async Task GetRecipeById_WhenRecipeHasNoImages_ReturnsEmptyImagesArray()
    {
        using var response = await _httpClient.GetAsync($"/recipes/{RecipeListingTestData.MatchaCloudId}");
        var payload = await response.Content.ReadFromJsonAsync<RecipeDetailDto>(JsonOptions);

        Assert.NotNull(payload);
        Assert.Empty(payload.Images);
    }

    [Fact]
    public async Task GetRecipeById_WhenRecipeDoesNotExist_ReturnsNotFound()
    {
        using var response = await _httpClient.GetAsync($"/recipes/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetRecipeById_WhenRouteIdIsInvalid_ReturnsNotFound()
    {
        using var response = await _httpClient.GetAsync("/recipes/not-a-guid");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}

public sealed class RecipeListingApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("Persistence__UseInMemory", "true");
        Environment.SetEnvironmentVariable("Persistence__InMemoryDatabaseName", "recipe-listing-api-tests");

        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, configurationBuilder) =>
        {
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Persistence:UseInMemory"] = "true",
                ["Persistence:InMemoryDatabaseName"] = "recipe-listing-api-tests",
            });
        });

        builder.ConfigureServices(services =>
        {
            using var scope = services.BuildServiceProvider().CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CoffeeCodexDbContext>();

            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            RecipeListingTestData.SeedAsync(dbContext).GetAwaiter().GetResult();
        });
    }
}
