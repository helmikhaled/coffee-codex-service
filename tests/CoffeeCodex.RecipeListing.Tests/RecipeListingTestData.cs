using CoffeeCodex.Domain.Recipes;
using CoffeeCodex.Infrastructure.Persistence;

namespace CoffeeCodex.RecipeListing.Tests;

internal static class RecipeListingTestData
{
    public static async Task SeedAsync(CoffeeCodexDbContext dbContext)
    {
        if (dbContext.Recipes.Any())
        {
            return;
        }

        var now = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var authorA = new Author
        {
            Id = Guid.Parse("00000000-0000-0000-0000-0000000000a1"),
            Name = "Coffee Codex",
        };

        var authorB = new Author
        {
            Id = Guid.Parse("00000000-0000-0000-0000-0000000000b1"),
            Name = "Guest Barista",
        };

        var espressoTonic = new Recipe
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            Title = "Espresso Tonic",
            Description = "Bright espresso with tonic water and citrus.",
            Category = RecipeCategory.Citrus,
            Slug = "espresso-tonic",
            AuthorId = authorA.Id,
            BrewCount = 124,
            DisplayOrder = 1,
            CreatedAt = now,
            UpdatedAt = now,
        };

        var icedMapleLatte = new Recipe
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
            Title = "Iced Maple Latte",
            Description = "A cold latte with maple sweetness.",
            Category = RecipeCategory.Iced,
            Slug = "iced-maple-latte",
            AuthorId = authorB.Id,
            BrewCount = 86,
            DisplayOrder = 2,
            CreatedAt = now,
            UpdatedAt = now,
        };

        var matchaCloud = new Recipe
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
            Title = "Matcha Cloud",
            Description = "A soft iced matcha drink with no espresso.",
            Category = RecipeCategory.Modern,
            Slug = "matcha-cloud",
            AuthorId = authorA.Id,
            BrewCount = 43,
            DisplayOrder = 2,
            CreatedAt = now,
            UpdatedAt = now,
        };

        var affogatoFloat = new Recipe
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000004"),
            Title = "Affogato Float",
            Description = "Espresso poured over vanilla ice cream.",
            Category = RecipeCategory.Dessert,
            Slug = "affogato-float",
            AuthorId = authorB.Id,
            BrewCount = 65,
            DisplayOrder = 4,
            CreatedAt = now,
            UpdatedAt = now,
        };

        var brewSpecs = new[]
        {
            new RecipeBrewSpecs
            {
                RecipeId = espressoTonic.Id,
                CoffeeDoseInGrams = 18m,
                CoffeeYieldInGrams = 36m,
                CupSizeInMl = 250,
                Difficulty = DifficultyLevel.Beginner,
                TimeInMinutes = 5,
            },
            new RecipeBrewSpecs
            {
                RecipeId = icedMapleLatte.Id,
                CoffeeDoseInGrams = 18m,
                CoffeeYieldInGrams = 36m,
                MilkInMl = 180,
                CupSizeInMl = 300,
                Difficulty = DifficultyLevel.Intermediate,
                TimeInMinutes = 6,
            },
            new RecipeBrewSpecs
            {
                RecipeId = affogatoFloat.Id,
                CoffeeDoseInGrams = 18m,
                CoffeeYieldInGrams = 40m,
                CupSizeInMl = 220,
                Difficulty = DifficultyLevel.Advanced,
                TimeInMinutes = 7,
            },
        };

        var recipeImages = new[]
        {
            new RecipeImage
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                RecipeId = espressoTonic.Id,
                BlobUrl = "https://cdn.example.com/recipes/espresso-tonic-thumb.jpg",
                Caption = "Espresso tonic thumbnail",
                Position = 1,
                CreatedAt = now,
            },
            new RecipeImage
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                RecipeId = espressoTonic.Id,
                BlobUrl = "https://cdn.example.com/recipes/espresso-tonic-detail.jpg",
                Caption = "Espresso tonic detail",
                Position = 2,
                CreatedAt = now,
            },
            new RecipeImage
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                RecipeId = icedMapleLatte.Id,
                BlobUrl = "https://cdn.example.com/recipes/iced-maple-latte-thumb.jpg",
                Caption = "Iced maple latte thumbnail",
                Position = 1,
                CreatedAt = now,
            },
            new RecipeImage
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000004"),
                RecipeId = affogatoFloat.Id,
                BlobUrl = "https://cdn.example.com/recipes/affogato-float-thumb.jpg",
                Caption = "Affogato float thumbnail",
                Position = 1,
                CreatedAt = now,
            },
        };

        await dbContext.Authors.AddRangeAsync(authorA, authorB);
        await dbContext.Recipes.AddRangeAsync(espressoTonic, icedMapleLatte, matchaCloud, affogatoFloat);
        await dbContext.RecipeBrewSpecs.AddRangeAsync(brewSpecs);
        await dbContext.RecipeImages.AddRangeAsync(recipeImages);
        await dbContext.SaveChangesAsync();
    }
}
