using CoffeeCodex.Application.Recipes.Queries.GetRecipeDetail;
using CoffeeCodex.Application.Recipes.Queries.GetRecipes;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CoffeeCodex.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IValidator<GetRecipeDetailQuery>, GetRecipeDetailQueryValidator>();
        services.AddScoped<IGetRecipeDetailHandler, GetRecipeDetailHandler>();
        services.AddScoped<IValidator<GetRecipesQuery>, GetRecipesQueryValidator>();
        services.AddScoped<IGetRecipesHandler, GetRecipesHandler>();

        return services;
    }
}
