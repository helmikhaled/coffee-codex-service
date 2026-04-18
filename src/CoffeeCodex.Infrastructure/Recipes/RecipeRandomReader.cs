using CoffeeCodex.Application.Recipes.Queries.GetRandomRecipe;
using CoffeeCodex.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CoffeeCodex.Infrastructure.Recipes;

internal sealed class RecipeRandomReader(CoffeeCodexDbContext dbContext) : IRecipeRandomReader
{
    public async Task<RandomRecipeDto?> GetRandomRecipeAsync(
        GetRandomRecipeQuery query,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await dbContext.Recipes
            .AsNoTracking()
            .CountAsync(cancellationToken);

        if (totalCount == 0)
        {
            return null;
        }

        var offset = Random.Shared.Next(totalCount);

        return await dbContext.Recipes
            .AsNoTracking()
            .OrderBy(recipe => recipe.Id)
            .Skip(offset)
            .Take(1)
            .Select(recipe => new RandomRecipeDto(recipe.Id))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
