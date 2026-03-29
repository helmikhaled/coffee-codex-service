namespace CoffeeCodex.Domain.Recipes;

public sealed class Author
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<Recipe> Recipes { get; set; } = [];
}
