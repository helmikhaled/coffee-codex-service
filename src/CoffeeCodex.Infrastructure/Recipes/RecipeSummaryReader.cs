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
        var totalCount = await baseQuery.CountAsync(cancellationToken);
        var skip = (query.Page - 1) * query.PageSize;

        var items = await baseQuery
            .OrderBy(recipe => recipe.DisplayOrder)
            .ThenBy(recipe => recipe.Id)
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
