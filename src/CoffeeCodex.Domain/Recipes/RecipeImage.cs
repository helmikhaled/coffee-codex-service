namespace CoffeeCodex.Domain.Recipes;

public sealed class RecipeImage
{
    public Guid Id { get; set; }

    public Guid RecipeId { get; set; }

    public string BlobUrl { get; set; } = string.Empty;

    public string? Caption { get; set; }

    public int Position { get; set; }

    public DateTime CreatedAt { get; set; }

    public Recipe Recipe { get; set; } = null!;
}
