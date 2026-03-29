using CoffeeCodex.Application.Recipes.Queries.GetRecipes;

namespace CoffeeCodex.API.Recipes;

public sealed record GetRecipesRequest(
    int Page = RecipeListingDefaults.DefaultPage,
    int PageSize = RecipeListingDefaults.DefaultPageSize)
{
    public GetRecipesQuery ToQuery() => new(Page, PageSize);
}
