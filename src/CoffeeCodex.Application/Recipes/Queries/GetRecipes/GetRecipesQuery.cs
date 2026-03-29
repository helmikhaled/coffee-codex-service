namespace CoffeeCodex.Application.Recipes.Queries.GetRecipes;

public sealed record GetRecipesQuery(
    int Page = RecipeListingDefaults.DefaultPage,
    int PageSize = RecipeListingDefaults.DefaultPageSize);
