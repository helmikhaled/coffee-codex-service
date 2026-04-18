namespace CoffeeCodex.Application.Recipes.Queries.GetRandomRecipe;

public interface IRecipeRandomReader
{
    Task<RandomRecipeDto?> GetRandomRecipeAsync(
        GetRandomRecipeQuery query,
        CancellationToken cancellationToken = default);
}
