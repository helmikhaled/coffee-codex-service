using CoffeeCodex.Application.Recipes.Commands.RecordRecipeView;
using CoffeeCodex.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CoffeeCodex.Infrastructure.Recipes;

internal sealed class RecipeViewRecorder(CoffeeCodexDbContext dbContext) : IRecipeViewRecorder
{
    public async Task<bool> RecordViewAsync(
        RecordRecipeViewCommand command,
        CancellationToken cancellationToken = default)
    {
        if (dbContext.Database.IsRelational())
        {
            var affectedRows = await dbContext.Recipes
                .Where(recipe => recipe.Id == command.Id)
                .ExecuteUpdateAsync(
                    setters => setters.SetProperty(
                        recipe => recipe.BrewCount,
                        recipe => recipe.BrewCount + 1),
                    cancellationToken);

            return affectedRows == 1;
        }

        var recipe = await dbContext.Recipes
            .SingleOrDefaultAsync(recipe => recipe.Id == command.Id, cancellationToken);

        if (recipe is null)
        {
            return false;
        }

        recipe.BrewCount += 1;
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}
