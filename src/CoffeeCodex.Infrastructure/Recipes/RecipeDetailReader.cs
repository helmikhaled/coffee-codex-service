using CoffeeCodex.Application.Recipes.Queries.GetRecipeDetail;
using CoffeeCodex.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CoffeeCodex.Infrastructure.Recipes;

internal sealed class RecipeDetailReader(CoffeeCodexDbContext dbContext) : IRecipeDetailReader
{
    public async Task<RecipeDetailDto?> GetRecipeDetailAsync(
        GetRecipeDetailQuery query,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Recipes
            .AsNoTracking()
            .AsSplitQuery()
            .Where(recipe => recipe.Id == query.Id)
            .Select(recipe => new RecipeDetailDto(
                recipe.Id,
                recipe.Slug,
                recipe.Title,
                recipe.Description,
                recipe.Category,
                recipe.BrewCount,
                new AuthorDto(
                    recipe.Author.Id,
                    recipe.Author.Name,
                    recipe.Author.AvatarUrl),
                recipe.BrewSpecs == null
                    ? null
                    : new BrewSpecsDto(
                        recipe.BrewSpecs.CoffeeDoseInGrams,
                        recipe.BrewSpecs.CoffeeYieldInGrams,
                        recipe.BrewSpecs.MilkInMl,
                        recipe.BrewSpecs.CupSizeInMl,
                        recipe.BrewSpecs.Difficulty,
                        recipe.BrewSpecs.TimeInMinutes),
                recipe.Ingredients
                    .OrderBy(ingredient => ingredient.Position)
                    .Select(ingredient => new IngredientDto(
                        ingredient.Name,
                        ingredient.QuantityValue,
                        ingredient.Unit,
                        ingredient.Position))
                    .ToList(),
                recipe.Steps
                    .OrderBy(step => step.StepNumber)
                    .Select(step => new StepDto(
                        step.StepNumber,
                        step.Instruction))
                    .ToList(),
                recipe.Images
                    .OrderBy(image => image.Position)
                    .ThenBy(image => image.Id)
                    .Select(image => new ImageDto(
                        image.BlobUrl,
                        image.Caption,
                        image.Position))
                    .ToList(),
                recipe.RecipeTags
                    .Select(recipeTag => recipeTag.Tag.Name)
                    .OrderBy(tagName => tagName)
                    .ToList()))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
