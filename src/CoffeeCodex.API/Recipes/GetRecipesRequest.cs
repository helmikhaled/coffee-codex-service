using CoffeeCodex.Application.Recipes.Queries.GetRecipes;

namespace CoffeeCodex.API.Recipes;

public sealed record GetRecipesRequest(
    int Page = RecipeListingDefaults.DefaultPage,
    int PageSize = RecipeListingDefaults.DefaultPageSize,
    string? Category = null,
    string[]? Tag = null)
{
    public GetRecipesQuery ToQuery()
    {
        var normalizedTags = Tag?
            .Select(value => value?.Trim() ?? string.Empty)
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        return new GetRecipesQuery(
            Page,
            PageSize,
            Category,
            normalizedTags);
    }
}
