using CoffeeCodex.Shared.Pagination;

namespace CoffeeCodex.Application.Recipes.Queries.GetRecipes;

public interface IRecipeSummaryReader
{
    Task<PagedResponse<RecipeSummaryDto>> GetRecipesAsync(
        GetRecipesQuery query,
        CancellationToken cancellationToken = default);
}
