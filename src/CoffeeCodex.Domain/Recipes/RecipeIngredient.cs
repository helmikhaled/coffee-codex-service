namespace CoffeeCodex.Domain.Recipes;

public sealed class RecipeIngredient
{
    public Guid Id { get; set; }

    public Guid RecipeId { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal QuantityValue { get; set; }

    public string Unit { get; set; } = string.Empty;

    public int Position { get; set; }

    public Recipe Recipe { get; set; } = null!;
}
