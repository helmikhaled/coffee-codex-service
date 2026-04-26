using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace CoffeeCodex.RecipeListing.Tests;

public sealed class AuthenticationConfigurationTest
{
    [Fact]
    public void CreateClient_WhenAuth0DomainMissing_ThrowsConfigurationError()
    {
        using var factory = new InvalidAuthenticationConfigurationApiFactory(
            new Dictionary<string, string?>
            {
                ["Auth0:Domain"] = string.Empty,
            });

        var exception = Assert.ThrowsAny<Exception>(() => factory.CreateClient());

        Assert.Contains("Auth0:Domain", exception.ToString());
    }

    [Fact]
    public void CreateClient_WhenAuth0AudienceMissing_ThrowsConfigurationError()
    {
        using var factory = new InvalidAuthenticationConfigurationApiFactory(
            new Dictionary<string, string?>
            {
                ["Auth0:Audience"] = string.Empty,
            });

        var exception = Assert.ThrowsAny<Exception>(() => factory.CreateClient());

        Assert.Contains("Auth0:Audience", exception.ToString());
    }

    [Fact]
    public async Task CreateClient_WhenAuth0ConfigurationIsValid_BootsSuccessfully()
    {
        await using var factory = new Api.RecipeListingApiFactory($"auth-config-valid-{Guid.NewGuid():N}");
        using var client = factory.CreateClient();
        using var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

internal sealed class InvalidAuthenticationConfigurationApiFactory : Api.RecipeListingApiFactory
{
    private readonly IReadOnlyDictionary<string, string?> _configurationOverrides;

    public InvalidAuthenticationConfigurationApiFactory(IReadOnlyDictionary<string, string?> configurationOverrides)
        : base($"auth-config-invalid-{Guid.NewGuid():N}")
    {
        _configurationOverrides = configurationOverrides;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureAppConfiguration((_, configurationBuilder) =>
        {
            configurationBuilder.AddInMemoryCollection(_configurationOverrides);
        });
    }
}
