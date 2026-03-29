using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
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
