namespace CoffeeCodex.Domain.Recipes;

public sealed class Recipe
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public RecipeCategory Category { get; set; }

    public string Slug { get; set; } = string.Empty;

    public Guid AuthorId { get; set; }

    public int BrewCount { get; set; }

    public int DisplayOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Author Author { get; set; } = null!;

    public RecipeBrewSpecs? BrewSpecs { get; set; }

    public ICollection<RecipeImage> Images { get; set; } = [];
}
