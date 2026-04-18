using CoffeeCodex.Application.Recipes.Commands.RecordRecipeView;
using FluentValidation;

namespace CoffeeCodex.RecipeListing.Tests.Application;

public sealed class RecordRecipeViewHandlerTest
{
    [Fact]
    public async Task HandleAsync_WhenRecipeExists_ReturnsTrue()
    {
        var handler = new RecordRecipeViewHandler(
            new RecordRecipeViewCommandValidator(),
            new FakeRecipeViewRecorder(_ => true));

        var result = await handler.HandleAsync(new RecordRecipeViewCommand(Guid.NewGuid()));

        Assert.True(result);
    }

    [Fact]
    public async Task HandleAsync_WhenRecipeDoesNotExist_ReturnsFalse()
    {
        var handler = new RecordRecipeViewHandler(
            new RecordRecipeViewCommandValidator(),
            new FakeRecipeViewRecorder(_ => false));

        var result = await handler.HandleAsync(new RecordRecipeViewCommand(Guid.NewGuid()));

        Assert.False(result);
    }

    [Fact]
    public async Task HandleAsync_WhenIdIsEmpty_ThrowsValidationException()
    {
        var handler = new RecordRecipeViewHandler(
            new RecordRecipeViewCommandValidator(),
            new FakeRecipeViewRecorder(_ => true));

        await Assert.ThrowsAsync<ValidationException>(
            () => handler.HandleAsync(new RecordRecipeViewCommand(Guid.Empty)));
    }

    private sealed class FakeRecipeViewRecorder(Func<RecordRecipeViewCommand, bool> selector)
        : IRecipeViewRecorder
    {
        public Task<bool> RecordViewAsync(
            RecordRecipeViewCommand command,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(selector(command));
        }
    }
}
