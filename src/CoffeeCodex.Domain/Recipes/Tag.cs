namespace CoffeeCodex.Domain.Recipes;

public sealed class Tag
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<RecipeTag> RecipeTags { get; set; } = [];
}
