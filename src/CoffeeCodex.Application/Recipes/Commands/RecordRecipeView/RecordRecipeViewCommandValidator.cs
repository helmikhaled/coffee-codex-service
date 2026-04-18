using FluentValidation;

namespace CoffeeCodex.Application.Recipes.Commands.RecordRecipeView;

public sealed class RecordRecipeViewCommandValidator : AbstractValidator<RecordRecipeViewCommand>
{
    public RecordRecipeViewCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEqual(Guid.Empty);
    }
}
