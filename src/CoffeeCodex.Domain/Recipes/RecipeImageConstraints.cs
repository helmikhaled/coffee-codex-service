namespace CoffeeCodex.Domain.Recipes;

public static class RecipeImageConstraints
{
    public const int MinPosition = 1;

    // No explicit upper bound is defined for image count yet.
    public const int MaxImagesPerRecipe = int.MaxValue;

    public const bool IsCaptionRequired = false;
}
