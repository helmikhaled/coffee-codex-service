using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoffeeCodex.Infrastructure.Persistence;

public static class DatabaseInitializationExtensions
{
    public static async Task InitializeDatabaseAsync(this IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CoffeeCodexDbContext>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var seedOnStartup = bool.TryParse(configuration["Persistence:SeedOnStartup"], out var parsedSeedOnStartup)
            && parsedSeedOnStartup;

        await dbContext.Database.EnsureCreatedAsync();

        if (seedOnStartup)
        {
            await RecipeListingSeedData.SeedAsync(dbContext);
        }
    }
}
