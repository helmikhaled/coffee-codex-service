using CoffeeCodex.Application.Recipes.Queries.GetRecipes;

namespace CoffeeCodex.API.Recipes;

public sealed record GetRecipesRequest(
    int Page = RecipeListingDefaults.DefaultPage,
    int PageSize = RecipeListingDefaults.DefaultPageSize,
    string? Category = null,
    string[]? Tag = null,
    string? Search = null)
{
    public GetRecipesQuery ToQuery()
    {
        var normalizedTags = Tag?
            .Select(value => value?.Trim() ?? string.Empty)
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        var normalizedSearch = string.IsNullOrWhiteSpace(Search)
            ? null
            : Search.Trim();

        return new GetRecipesQuery(
            Page,
            PageSize,
            Category,
            normalizedTags,
            normalizedSearch);
    }
}
