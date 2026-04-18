using CoffeeCodex.Application.Recipes.Queries.GetRecipeDetail;
using CoffeeCodex.Application.Recipes.Queries.GetRandomRecipe;
using CoffeeCodex.Application.Recipes.Queries.GetRecipes;
using CoffeeCodex.Infrastructure.Persistence;
using CoffeeCodex.Infrastructure.Recipes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoffeeCodex.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var useInMemory = bool.TryParse(configuration["Persistence:UseInMemory"], out var parsedUseInMemory)
            && parsedUseInMemory;
        var connectionString = configuration.GetConnectionString("PostgreSql");
        var inMemoryDatabaseName = configuration["Persistence:InMemoryDatabaseName"];

        services.AddDbContext<CoffeeCodexDbContext>(options =>
        {
            if (useInMemory)
            {
                if (string.IsNullOrWhiteSpace(inMemoryDatabaseName))
                {
                    throw new InvalidOperationException(
                        "Missing configuration value 'Persistence:InMemoryDatabaseName'.");
                }

                options.UseInMemoryDatabase(inMemoryDatabaseName);
                return;
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Missing connection string 'ConnectionStrings:PostgreSql'.");
            }

            options.UseNpgsql(connectionString);
        });
        services.AddScoped<IRecipeDetailReader, RecipeDetailReader>();
        services.AddScoped<IRecipeRandomReader, RecipeRandomReader>();
        services.AddScoped<IRecipeSummaryReader, RecipeSummaryReader>();

        return services;
    }
}
