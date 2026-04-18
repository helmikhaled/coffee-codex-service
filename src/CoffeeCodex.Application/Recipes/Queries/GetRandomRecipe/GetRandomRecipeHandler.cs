namespace CoffeeCodex.Application.Recipes.Queries.GetRandomRecipe;

public sealed class GetRandomRecipeHandler(IRecipeRandomReader recipeRandomReader) : IGetRandomRecipeHandler
{
    public Task<RandomRecipeDto?> HandleAsync(
        GetRandomRecipeQuery query,
        CancellationToken cancellationToken = default)
    {
        return recipeRandomReader.GetRandomRecipeAsync(query, cancellationToken);
    }
}
