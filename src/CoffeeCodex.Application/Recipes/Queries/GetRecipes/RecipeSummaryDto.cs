using CoffeeCodex.Domain.Recipes;

namespace CoffeeCodex.Application.Recipes.Queries.GetRecipes;

public sealed record RecipeSummaryDto(
    Guid Id,
    string Slug,
    string Title,
    RecipeCategory Category,
    string? ThumbnailUrl,
    int BrewCount,
    string AuthorName,
    DifficultyLevel? Difficulty);
