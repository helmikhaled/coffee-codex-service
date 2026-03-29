using System.Data;
using System.Globalization;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CoffeeCodex.Infrastructure.Persistence;

public static class DatabaseInitializationExtensions
{
    public static async Task InitializeDatabaseAsync(this IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CoffeeCodexDbContext>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger(typeof(DatabaseInitializationExtensions));

        var seedOnStartup = bool.TryParse(configuration["Persistence:SeedOnStartup"], out var parsed)
            && parsed;

        if (dbContext.Database.IsInMemory())
        {
            await dbContext.Database.EnsureCreatedAsync();
            logger.LogInformation("In-memory database ensured.");
        }
        else
        {
            var pendingMigrations = (await dbContext.Database.GetPendingMigrationsAsync()).ToArray();

            if (pendingMigrations.Length == 0)
            {
                logger.LogInformation("No pending migrations. Database is up-to-date.");
            }
            else
            {
                logger.LogInformation(
                    "Applying {Count} pending migration(s): {Migrations}",
                    pendingMigrations.Length,
                    string.Join(", ", pendingMigrations));

                await dbContext.Database.MigrateAsync();
            }
        }

        if (seedOnStartup)
        {
            await RecipeListingSeedData.SeedAsync(dbContext);
            logger.LogInformation("Seeding completed.");
        }
    }
}
