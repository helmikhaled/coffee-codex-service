using FluentValidation;

namespace CoffeeCodex.Application.Recipes.Queries.GetRecipes;

public sealed class GetRecipesQueryValidator : AbstractValidator<GetRecipesQuery>
{
    public GetRecipesQueryValidator()
    {
        RuleFor(query => query.Page)
            .GreaterThanOrEqualTo(RecipeListingDefaults.DefaultPage);

        RuleFor(query => query.PageSize)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(RecipeListingDefaults.MaxPageSize);
    }
}
