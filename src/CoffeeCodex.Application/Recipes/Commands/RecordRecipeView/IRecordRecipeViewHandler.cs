namespace CoffeeCodex.Application.Recipes.Commands.RecordRecipeView;

public interface IRecordRecipeViewHandler
{
    Task<bool> HandleAsync(
        RecordRecipeViewCommand command,
        CancellationToken cancellationToken = default);
}
