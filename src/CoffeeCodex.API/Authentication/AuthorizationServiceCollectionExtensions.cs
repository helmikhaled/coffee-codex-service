namespace CoffeeCodex.API.Authentication;

internal static class AuthorizationServiceCollectionExtensions
{
    public static IServiceCollection AddAdminAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthorizationPolicyNames.AdminOnly, policy =>
            {
                policy.RequireAuthenticatedUser();
            });
        });

        return services;
    }
}
