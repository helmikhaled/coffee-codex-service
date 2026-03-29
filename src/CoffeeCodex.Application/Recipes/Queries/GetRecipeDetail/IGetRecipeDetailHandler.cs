namespace CoffeeCodex.Application.Recipes.Queries.GetRecipeDetail;

public interface IGetRecipeDetailHandler
{
    Task<RecipeDetailDto?> HandleAsync(
        GetRecipeDetailQuery query,
        CancellationToken cancellationToken = default);
}
