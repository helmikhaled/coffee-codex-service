using CoffeeCodex.Domain.Recipes;

namespace CoffeeCodex.Application.Recipes.Queries.GetRecipeDetail;

public sealed record RecipeDetailDto(
    Guid Id,
    string Slug,
    string Title,
    string Description,
    RecipeCategory Category,
    int BrewCount,
    AuthorDto Author,
    BrewSpecsDto? BrewSpecs,
    IReadOnlyList<IngredientDto> Ingredients,
    IReadOnlyList<StepDto> Steps,
    IReadOnlyList<ImageDto> Images,
    IReadOnlyList<string> Tags);

public sealed record AuthorDto(
    Guid Id,
    string Name,
    string? AvatarUrl);

public sealed record BrewSpecsDto(
    decimal? CoffeeDoseInGrams,
    decimal? CoffeeYieldInGrams,
    int? MilkInMl,
    int? CupSizeInMl,
    DifficultyLevel Difficulty,
    int TimeInMinutes);

public sealed record IngredientDto(
    string Name,
    decimal QuantityValue,
    string Unit,
    int Position);

public sealed record StepDto(
    int StepNumber,
    string Instruction);

public sealed record ImageDto(
    string Url,
    string? Caption,
    int Position);
