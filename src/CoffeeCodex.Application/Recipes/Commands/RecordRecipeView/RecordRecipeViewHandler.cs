using FluentValidation;

namespace CoffeeCodex.Application.Recipes.Commands.RecordRecipeView;

public sealed class RecordRecipeViewHandler(
    IValidator<RecordRecipeViewCommand> validator,
    IRecipeViewRecorder recipeViewRecorder) : IRecordRecipeViewHandler
{
    public async Task<bool> HandleAsync(
        RecordRecipeViewCommand command,
        CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);

        return await recipeViewRecorder.RecordViewAsync(command, cancellationToken);
    }
}
