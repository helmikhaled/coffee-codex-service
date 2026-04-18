namespace CoffeeCodex.Application.Recipes.Queries.GetRandomRecipe;

public interface IGetRandomRecipeHandler
{
    Task<RandomRecipeDto?> HandleAsync(
        GetRandomRecipeQuery query,
        CancellationToken cancellationToken = default);
}
