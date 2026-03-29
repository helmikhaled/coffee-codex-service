namespace CoffeeCodex.Domain.Recipes;

public sealed class RecipeBrewSpecs
{
    public Guid RecipeId { get; set; }

    public decimal? CoffeeDoseInGrams { get; set; }

    public decimal? CoffeeYieldInGrams { get; set; }

    public int? MilkInMl { get; set; }

    public int? CupSizeInMl { get; set; }

    public DifficultyLevel Difficulty { get; set; }

    public int TimeInMinutes { get; set; }

    public Recipe Recipe { get; set; } = null!;
}
