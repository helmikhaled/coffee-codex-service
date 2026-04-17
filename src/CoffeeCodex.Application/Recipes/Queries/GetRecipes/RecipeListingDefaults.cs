namespace CoffeeCodex.Application.Recipes.Queries.GetRecipes;

public static class RecipeListingDefaults
{
    public const int DefaultPage = 1;
    public const int DefaultPageSize = 12;
    public const int MaxPageSize = 50;
    public const int MaxSearchLength = 100;
    public const string PrimarySort = "display_order ASC";
    public const string SecondarySort = "id ASC";
}
