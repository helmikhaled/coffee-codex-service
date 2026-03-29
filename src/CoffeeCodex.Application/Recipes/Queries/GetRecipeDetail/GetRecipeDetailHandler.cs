using FluentValidation;

namespace CoffeeCodex.Application.Recipes.Queries.GetRecipeDetail;

public sealed class GetRecipeDetailHandler(
    IValidator<GetRecipeDetailQuery> validator,
    IRecipeDetailReader recipeDetailReader) : IGetRecipeDetailHandler
{
    public async Task<RecipeDetailDto?> HandleAsync(
        GetRecipeDetailQuery query,
        CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(query, cancellationToken);

        return await recipeDetailReader.GetRecipeDetailAsync(query, cancellationToken);
    }
}
