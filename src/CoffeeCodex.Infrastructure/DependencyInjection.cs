using CoffeeCodex.Infrastructure.Persistence;
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
        var connectionString = configuration.GetConnectionString("PostgreSql")
            ?? "Host=localhost;Port=5432;Database=coffee_codex;Username=postgres;Password=postgres";

        services.AddDbContext<CoffeeCodexDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        return services;
    }
}
