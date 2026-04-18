using CoffeeCodex.Application.Recipes.Queries.GetRandomRecipe;

namespace CoffeeCodex.RecipeListing.Tests.Application;

public sealed class GetRandomRecipeHandlerTest
{
    [Fact]
    public async Task HandleAsync_WhenReaderReturnsRecipe_ReturnsRecipe()
    {
        var expected = new RandomRecipeDto(Guid.NewGuid());
        var handler = new GetRandomRecipeHandler(new FakeRecipeRandomReader(_ => expected));

        var result = await handler.HandleAsync(new GetRandomRecipeQuery());

        Assert.NotNull(result);
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task HandleAsync_WhenReaderReturnsNull_ReturnsNull()
    {
        var handler = new GetRandomRecipeHandler(new FakeRecipeRandomReader(_ => null));

        var result = await handler.HandleAsync(new GetRandomRecipeQuery());

        Assert.Null(result);
    }

    private sealed class FakeRecipeRandomReader(Func<GetRandomRecipeQuery, RandomRecipeDto?> selector)
        : IRecipeRandomReader
    {
        public Task<RandomRecipeDto?> GetRandomRecipeAsync(
            GetRandomRecipeQuery query,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(selector(query));
        }
    }
}
