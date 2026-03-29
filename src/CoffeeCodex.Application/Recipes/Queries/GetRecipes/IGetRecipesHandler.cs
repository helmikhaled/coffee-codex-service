using CoffeeCodex.Shared.Pagination;

namespace CoffeeCodex.Application.Recipes.Queries.GetRecipes;

public interface IGetRecipesHandler
{
    Task<PagedResponse<RecipeSummaryDto>> HandleAsync(
        GetRecipesQuery query,
        CancellationToken cancellationToken = default);
}
