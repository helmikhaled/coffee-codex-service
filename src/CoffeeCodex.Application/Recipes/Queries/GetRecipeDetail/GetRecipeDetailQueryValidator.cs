using FluentValidation;

namespace CoffeeCodex.Application.Recipes.Queries.GetRecipeDetail;

public sealed class GetRecipeDetailQueryValidator : AbstractValidator<GetRecipeDetailQuery>
{
    public GetRecipeDetailQueryValidator()
    {
        RuleFor(query => query.Id)
            .NotEqual(Guid.Empty);
    }
}
