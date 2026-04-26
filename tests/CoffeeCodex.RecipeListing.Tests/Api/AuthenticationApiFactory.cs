using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace CoffeeCodex.RecipeListing.Tests.Api;

public sealed class AuthenticationApiFactory : RecipeListingApiFactory
{
    public AuthenticationApiFactory()
        : base($"authentication-api-tests-{Guid.NewGuid():N}")
    {
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureAppConfiguration((_, configurationBuilder) =>
        {
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Auth0:Domain"] = AuthTestTokenFactory.Issuer,
                ["Auth0:Audience"] = AuthTestTokenFactory.Audience,
            });
        });

        builder.ConfigureTestServices(services =>
        {
            services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = null;
                options.MetadataAddress = null;
                options.RequireHttpsMetadata = false;
                options.ConfigurationManager = new StaticConfigurationManager<OpenIdConnectConfiguration>(
                    new OpenIdConnectConfiguration
                    {
                        Issuer = AuthTestTokenFactory.Issuer,
                    });
                options.TokenValidationParameters.ValidIssuer = AuthTestTokenFactory.Issuer;
                options.TokenValidationParameters.ValidAudience = AuthTestTokenFactory.Audience;
                options.TokenValidationParameters.IssuerSigningKey = AuthTestTokenFactory.ValidationKey;
            });
        });
    }
}
