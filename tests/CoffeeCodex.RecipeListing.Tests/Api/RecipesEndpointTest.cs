using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CoffeeCodex.Application.Recipes.Queries.GetRecipeDetail;
using CoffeeCodex.Application.Recipes.Queries.GetRandomRecipe;
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
    public async Task GetRecipes_WhenSearchedByTitle_ReturnsMatchingRecipes()
    {
        using var response = await _httpClient.GetAsync("/recipes?search=affogato&page=1&pageSize=12");
        var payload = await response.Content.ReadFromJsonAsync<PagedResponse<RecipeSummaryDto>>(JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.Equal(1, payload.TotalCount);
        Assert.Single(payload.Items);
        Assert.Equal("affogato-float", payload.Items[0].Slug);
    }

    [Fact]
    public async Task GetRecipes_WhenSearchedByTagName_ReturnsMatchingRecipes()
    {
        using var response = await _httpClient.GetAsync("/recipes?search=sparkling&page=1&pageSize=12");
        var payload = await response.Content.ReadFromJsonAsync<PagedResponse<RecipeSummaryDto>>(JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.Equal(1, payload.TotalCount);
        Assert.Single(payload.Items);
        Assert.Equal("espresso-tonic", payload.Items[0].Slug);
    }

    [Fact]
    public async Task GetRecipes_WhenSearchedByIngredientName_ReturnsMatchingRecipes()
    {
        using var response = await _httpClient.GetAsync("/recipes?search=maple%20syrup&page=1&pageSize=12");
        var payload = await response.Content.ReadFromJsonAsync<PagedResponse<RecipeSummaryDto>>(JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.Equal(1, payload.TotalCount);
        Assert.Single(payload.Items);
        Assert.Equal("iced-maple-latte", payload.Items[0].Slug);
    }

    [Fact]
    public async Task GetRecipes_WhenSearchIsCaseInsensitive_ReturnsMatchingRecipes()
    {
        using var response = await _httpClient.GetAsync("/recipes?search=MaTcHa&page=1&pageSize=12");
        var payload = await response.Content.ReadFromJsonAsync<PagedResponse<RecipeSummaryDto>>(JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.Equal(2, payload.TotalCount);
        Assert.Collection(
            payload.Items,
            item => Assert.Equal("matcha-cloud", item.Slug),
            item => Assert.Equal("iced-maple-latte", item.Slug));
    }

    [Fact]
    public async Task GetRecipes_WhenSearchExceedsMaxLength_ReturnsBadRequest()
    {
        var overMaxSearch = new string('a', RecipeListingDefaults.MaxSearchLength + 1);
        using var response = await _httpClient.GetAsync(
            $"/recipes?search={Uri.EscapeDataString(overMaxSearch)}&page=1&pageSize=12");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetRecipes_WhenSearchAndFiltersAreCombined_ReturnsMatchingRecipes()
    {
        using var response = await _httpClient.GetAsync(
            "/recipes?search=matcha&category=Iced&tag=iced&page=1&pageSize=12");
        var payload = await response.Content.ReadFromJsonAsync<PagedResponse<RecipeSummaryDto>>(JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.Equal(1, payload.TotalCount);
        Assert.Single(payload.Items);
        Assert.Equal("iced-maple-latte", payload.Items[0].Slug);
    }

    [Fact]
    public async Task GetRandomRecipe_WhenRecipesExist_ReturnsOkAndResolvableRecipeId()
    {
        using var response = await _httpClient.GetAsync("/recipes/random");
        var payload = await response.Content.ReadFromJsonAsync<RandomRecipeDto>(JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(payload);
        Assert.NotEqual(Guid.Empty, payload.Id);

        using var detailResponse = await _httpClient.GetAsync($"/recipes/{payload.Id}");
        Assert.Equal(HttpStatusCode.OK, detailResponse.StatusCode);
    }

    [Fact]
    public async Task GetRandomRecipe_WhenNoRecipesExist_ReturnsNotFound()
    {
        await using var factory = new EmptyRecipeListingApiFactory();
        using var client = factory.CreateClient();
        using var response = await client.GetAsync("/recipes/random");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RecordRecipeView_WhenRecipeExists_ReturnsNoContent()
    {
        await using var factory = new RecipeListingApiFactory($"recipe-view-success-{Guid.NewGuid():N}");
        using var client = factory.CreateClient();
        using var response = await client.PostAsync(
            $"/recipes/{RecipeListingTestData.EspressoTonicId}/view",
            content: null);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task RecordRecipeView_WhenRecipeDoesNotExist_ReturnsNotFound()
    {
        await using var factory = new RecipeListingApiFactory($"recipe-view-missing-{Guid.NewGuid():N}");
        using var client = factory.CreateClient();
        using var response = await client.PostAsync(
            $"/recipes/{Guid.NewGuid()}/view",
            content: null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RecordRecipeView_WhenRecipeExists_DetailEndpointReturnsIncrementedBrewCount()
    {
        await using var factory = new RecipeListingApiFactory($"recipe-view-detail-{Guid.NewGuid():N}");
        using var client = factory.CreateClient();

        using var postResponse = await client.PostAsync(
            $"/recipes/{RecipeListingTestData.EspressoTonicId}/view",
            content: null);
        using var detailResponse = await client.GetAsync($"/recipes/{RecipeListingTestData.EspressoTonicId}");
        var payload = await detailResponse.Content.ReadFromJsonAsync<RecipeDetailDto>(JsonOptions);

        Assert.Equal(HttpStatusCode.NoContent, postResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, detailResponse.StatusCode);
        Assert.NotNull(payload);
        Assert.Equal(125, payload.BrewCount);
    }

    [Fact]
    public async Task RecordRecipeView_WhenRecipeExists_ListEndpointReturnsIncrementedBrewCount()
    {
        await using var factory = new RecipeListingApiFactory($"recipe-view-list-{Guid.NewGuid():N}");
        using var client = factory.CreateClient();

        using var postResponse = await client.PostAsync(
            $"/recipes/{RecipeListingTestData.EspressoTonicId}/view",
            content: null);
        using var listResponse = await client.GetAsync("/recipes?page=1&pageSize=12");
        var payload = await listResponse.Content.ReadFromJsonAsync<PagedResponse<RecipeSummaryDto>>(JsonOptions);
        var recipe = payload?.Items.Single(item => item.Id == RecipeListingTestData.EspressoTonicId);

        Assert.Equal(HttpStatusCode.NoContent, postResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        Assert.NotNull(recipe);
        Assert.Equal(125, recipe.BrewCount);
    }

    [Fact]
    public async Task RecordRecipeView_WhenCalledTwice_DetailEndpointReturnsCumulativeBrewCount()
    {
        await using var factory = new RecipeListingApiFactory($"recipe-view-cumulative-{Guid.NewGuid():N}");
        using var client = factory.CreateClient();

        using var firstResponse = await client.PostAsync(
            $"/recipes/{RecipeListingTestData.EspressoTonicId}/view",
            content: null);
        using var secondResponse = await client.PostAsync(
            $"/recipes/{RecipeListingTestData.EspressoTonicId}/view",
            content: null);
        using var detailResponse = await client.GetAsync($"/recipes/{RecipeListingTestData.EspressoTonicId}");
        var payload = await detailResponse.Content.ReadFromJsonAsync<RecipeDetailDto>(JsonOptions);

        Assert.Equal(HttpStatusCode.NoContent, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, secondResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, detailResponse.StatusCode);
        Assert.NotNull(payload);
        Assert.Equal(126, payload.BrewCount);
    }

    [Fact]
    public async Task RecordRecipeView_WhenIdIsEmpty_ReturnsBadRequest()
    {
        using var response = await _httpClient.PostAsync(
            "/recipes/00000000-0000-0000-0000-000000000000/view",
            content: null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
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

public class RecipeListingApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName;
    private readonly bool _seedData;

    public RecipeListingApiFactory()
        : this("recipe-listing-api-tests")
    {
    }

    internal RecipeListingApiFactory(string databaseName, bool seedData = true)
    {
        _databaseName = databaseName;
        _seedData = seedData;
    }

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("Persistence__UseInMemory", "true");
        Environment.SetEnvironmentVariable("Persistence__InMemoryDatabaseName", _databaseName);

        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, configurationBuilder) =>
        {
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Persistence:UseInMemory"] = "true",
                ["Persistence:InMemoryDatabaseName"] = _databaseName,
            });
        });

        builder.ConfigureServices(services =>
        {
            using var scope = services.BuildServiceProvider().CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CoffeeCodexDbContext>();

            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            if (_seedData)
            {
                RecipeListingTestData.SeedAsync(dbContext).GetAwaiter().GetResult();
            }
        });
    }
}

public sealed class EmptyRecipeListingApiFactory : RecipeListingApiFactory
{
    public EmptyRecipeListingApiFactory()
        : base("recipe-listing-api-tests-empty", seedData: false)
    {
    }
}
