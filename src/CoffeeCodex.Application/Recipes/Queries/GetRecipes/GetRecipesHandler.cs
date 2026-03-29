using CoffeeCodex.Shared.Pagination;
using FluentValidation;

namespace CoffeeCodex.Application.Recipes.Queries.GetRecipes;

public sealed class GetRecipesHandler(
    IValidator<GetRecipesQuery> validator,
    IRecipeSummaryReader recipeSummaryReader) : IGetRecipesHandler
{
    public async Task<PagedResponse<RecipeSummaryDto>> HandleAsync(
        GetRecipesQuery query,
        CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(query, cancellationToken);

        return await recipeSummaryReader.GetRecipesAsync(query, cancellationToken);
    }
}
