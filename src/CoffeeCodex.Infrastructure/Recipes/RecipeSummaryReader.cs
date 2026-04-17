using CoffeeCodex.Application.Recipes.Queries.GetRecipes;
using CoffeeCodex.Domain.Recipes;
using CoffeeCodex.Infrastructure.Persistence;
using CoffeeCodex.Shared.Pagination;
using Microsoft.EntityFrameworkCore;

namespace CoffeeCodex.Infrastructure.Recipes;

internal sealed class RecipeSummaryReader(CoffeeCodexDbContext dbContext) : IRecipeSummaryReader
{
    public async Task<PagedResponse<RecipeSummaryDto>> GetRecipesAsync(
        GetRecipesQuery query,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = dbContext.Recipes.AsNoTracking();

        if (query.Category is not null)
        {
            var category = Enum.Parse<RecipeCategory>(query.Category, ignoreCase: false);
            baseQuery = baseQuery.Where(recipe => recipe.Category == category);
        }

        var normalizedTags = query.Tags?
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Select(tag => tag.Trim())
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        if (normalizedTags is { Length: > 0 })
        {
            baseQuery = baseQuery.Where(recipe =>
                recipe.RecipeTags.Any(recipeTag => normalizedTags.Contains(recipeTag.Tag.Name)));
        }

        var normalizedSearch = query.Search?.Trim();
        var hasSearch = !string.IsNullOrWhiteSpace(normalizedSearch);
        if (hasSearch)
        {
            if (dbContext.Database.IsNpgsql())
            {
                var pattern = $"%{normalizedSearch}%";
                baseQuery = baseQuery.Where(recipe =>
                    EF.Functions.ILike(recipe.Title, pattern)
                    || recipe.RecipeTags.Any(recipeTag =>
                        EF.Functions.ILike(recipeTag.Tag.Name, pattern))
                    || recipe.Ingredients.Any(ingredient =>
                        EF.Functions.ILike(ingredient.Name, pattern)));
            }
            else
            {
                var loweredSearch = normalizedSearch.ToLowerInvariant();
                baseQuery = baseQuery.Where(recipe =>
                    recipe.Title.ToLower().Contains(loweredSearch)
                    || recipe.RecipeTags.Any(recipeTag =>
                        recipeTag.Tag.Name.ToLower().Contains(loweredSearch))
                    || recipe.Ingredients.Any(ingredient =>
                        ingredient.Name.ToLower().Contains(loweredSearch)));
            }
        }

        var totalCount = await baseQuery.CountAsync(cancellationToken);
        var skip = (query.Page - 1) * query.PageSize;

        IQueryable<Recipe> orderedQuery = baseQuery;
        if (hasSearch)
        {
            if (dbContext.Database.IsNpgsql())
            {
                var pattern = $"%{normalizedSearch}%";
                orderedQuery = orderedQuery
                    .OrderBy(recipe => EF.Functions.ILike(recipe.Title, pattern) ? 0 : 1)
                    .ThenBy(recipe => recipe.DisplayOrder)
                    .ThenBy(recipe => recipe.Id);
            }
            else
            {
                var loweredSearch = normalizedSearch.ToLowerInvariant();
                orderedQuery = orderedQuery
                    .OrderBy(recipe => recipe.Title.ToLower().Contains(loweredSearch) ? 0 : 1)
                    .ThenBy(recipe => recipe.DisplayOrder)
                    .ThenBy(recipe => recipe.Id);
            }
        }
        else
        {
            orderedQuery = orderedQuery
                .OrderBy(recipe => recipe.DisplayOrder)
                .ThenBy(recipe => recipe.Id);
        }

        var items = await orderedQuery
            .Skip(skip)
            .Take(query.PageSize)
            .Select(recipe => new RecipeSummaryDto(
                recipe.Id,
                recipe.Slug,
                recipe.Title,
                recipe.Category,
                recipe.Images
                    .Where(image => image.Position == 1)
                    .Select(image => image.BlobUrl)
                    .FirstOrDefault(),
                recipe.BrewCount,
                recipe.Author.Name,
                recipe.BrewSpecs != null
                    ? recipe.BrewSpecs.Difficulty
                    : (DifficultyLevel?)null))
            .ToListAsync(cancellationToken);

        return new PagedResponse<RecipeSummaryDto>(
            items,
            query.Page,
            query.PageSize,
            totalCount);
    }
}
