using CoffeeCodex.Application.Recipes.Queries.GetRecipeDetail;

namespace CoffeeCodex.RecipeListing.Tests.Application;

public sealed class GetRecipeDetailQueryValidatorTest
{
    private readonly GetRecipeDetailQueryValidator _validator = new();

    [Fact]
    public void Validate_WhenIdIsEmpty_ReturnsError()
    {
        var result = _validator.Validate(new GetRecipeDetailQuery(Guid.Empty));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(GetRecipeDetailQuery.Id));
    }

    [Fact]
    public void Validate_WhenIdIsValid_ReturnsNoErrors()
    {
        var result = _validator.Validate(new GetRecipeDetailQuery(Guid.NewGuid()));

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
}
