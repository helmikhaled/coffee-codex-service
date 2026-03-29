namespace CoffeeCodex.Application.Recipes.Queries.GetRecipeDetail;

public interface IRecipeDetailReader
{
    Task<RecipeDetailDto?> GetRecipeDetailAsync(
        GetRecipeDetailQuery query,
        CancellationToken cancellationToken = default);
}
