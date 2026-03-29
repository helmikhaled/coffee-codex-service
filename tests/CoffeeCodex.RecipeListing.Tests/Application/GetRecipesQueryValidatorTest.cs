using CoffeeCodex.Application.Recipes.Queries.GetRecipes;

namespace CoffeeCodex.RecipeListing.Tests.Application;

public sealed class GetRecipesQueryValidatorTest
{
    private readonly GetRecipesQueryValidator _validator = new();

    [Fact]
    public void Validate_WhenPageIsLessThanOne_ReturnsError()
    {
        var result = _validator.Validate(new GetRecipesQuery(Page: 0, PageSize: 12));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(GetRecipesQuery.Page));
    }

    [Fact]
    public void Validate_WhenPageSizeExceedsMax_ReturnsError()
    {
        var result = _validator.Validate(new GetRecipesQuery(Page: 1, PageSize: RecipeListingDefaults.MaxPageSize + 1));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(GetRecipesQuery.PageSize));
    }
}
