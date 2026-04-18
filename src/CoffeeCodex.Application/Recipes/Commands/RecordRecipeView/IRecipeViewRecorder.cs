namespace CoffeeCodex.Application.Recipes.Commands.RecordRecipeView;

public interface IRecipeViewRecorder
{
    Task<bool> RecordViewAsync(
        RecordRecipeViewCommand command,
        CancellationToken cancellationToken = default);
}
