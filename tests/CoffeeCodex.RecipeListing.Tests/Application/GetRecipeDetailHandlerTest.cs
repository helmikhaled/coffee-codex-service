using CoffeeCodex.Application.Recipes.Queries.GetRecipeDetail;
using CoffeeCodex.Domain.Recipes;
using FluentValidation;

namespace CoffeeCodex.RecipeListing.Tests.Application;

public sealed class GetRecipeDetailHandlerTest
{
    [Fact]
    public async Task HandleAsync_WhenRecipeExists_ReturnsRecipeDetail()
    {
        var recipeId = Guid.NewGuid();
        var expected = CreateRecipeDetail(recipeId);
        var handler = new GetRecipeDetailHandler(
            new GetRecipeDetailQueryValidator(),
            new FakeRecipeDetailReader(_ => expected));

        var result = await handler.HandleAsync(new GetRecipeDetailQuery(recipeId));

        Assert.NotNull(result);
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task HandleAsync_WhenRecipeDoesNotExist_ReturnsNull()
    {
        var handler = new GetRecipeDetailHandler(
            new GetRecipeDetailQueryValidator(),
            new FakeRecipeDetailReader(_ => null));

        var result = await handler.HandleAsync(new GetRecipeDetailQuery(Guid.NewGuid()));

        Assert.Null(result);
    }

    [Fact]
    public async Task HandleAsync_WhenIdIsEmpty_ThrowsValidationException()
    {
        var handler = new GetRecipeDetailHandler(
            new GetRecipeDetailQueryValidator(),
            new FakeRecipeDetailReader(_ => null));

        await Assert.ThrowsAsync<ValidationException>(
            () => handler.HandleAsync(new GetRecipeDetailQuery(Guid.Empty)));
    }

    private static RecipeDetailDto CreateRecipeDetail(Guid recipeId)
        => new(
            recipeId,
            "espresso-tonic",
            "Espresso Tonic",
            "Bright espresso with tonic water and citrus.",
            RecipeCategory.Citrus,
            124,
            new AuthorDto(Guid.NewGuid(), "Coffee Codex", "https://cdn.coffeecodex.dev/authors/coffee-codex.jpg"),
            new BrewSpecsDto(18m, 36m, null, 250, DifficultyLevel.Beginner, 5),
            [new IngredientDto("Espresso", 18m, "g", 1)],
            [new StepDto(1, "Pull espresso.")],
            [new ImageDto("https://cdn.example.com/espresso-tonic.jpg", "Hero", 1)],
            ["citrus", "sparkling"]);

    private sealed class FakeRecipeDetailReader(Func<GetRecipeDetailQuery, RecipeDetailDto?> selector)
        : IRecipeDetailReader
    {
        public Task<RecipeDetailDto?> GetRecipeDetailAsync(
            GetRecipeDetailQuery query,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(selector(query));
        }
    }
}
