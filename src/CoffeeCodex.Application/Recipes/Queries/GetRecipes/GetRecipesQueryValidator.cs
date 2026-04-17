using FluentValidation;
using CoffeeCodex.Domain.Recipes;

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

        RuleFor(query => query.Category)
            .Must(category => category is null
                || Enum.TryParse<RecipeCategory>(category, ignoreCase: false, out _))
            .WithMessage("Category must match a RecipeCategory value with case-sensitive formatting.");

        RuleForEach(query => query.Tags)
            .Must(tag => !string.IsNullOrWhiteSpace(tag))
            .WithMessage("Tag values must not be empty.");

        RuleFor(query => query.Search)
            .Must(search => search is null
                || search.Trim().Length <= RecipeListingDefaults.MaxSearchLength)
            .WithMessage(
                $"Search must be at most {RecipeListingDefaults.MaxSearchLength} characters.");
    }
}
