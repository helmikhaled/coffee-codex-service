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

    [Fact]
    public void Validate_WhenCategoryCasingIsInvalid_ReturnsError()
    {
        var result = _validator.Validate(new GetRecipesQuery(Page: 1, PageSize: 12, Category: "modern"));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(GetRecipesQuery.Category));
    }

    [Fact]
    public void Validate_WhenCategoryIsValid_PassesValidation()
    {
        var result = _validator.Validate(new GetRecipesQuery(Page: 1, PageSize: 12, Category: "Modern"));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WhenTagIsEmptyOrWhitespace_ReturnsError()
    {
        var result = _validator.Validate(new GetRecipesQuery(
            Page: 1,
            PageSize: 12,
            Tags: ["matcha", "  "]));

        Assert.False(result.IsValid);
        Assert.Contains(
            result.Errors,
            error => error.PropertyName.StartsWith(nameof(GetRecipesQuery.Tags), StringComparison.Ordinal));
    }
}
