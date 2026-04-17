using CoffeeCodex.Domain.Recipes;
using CoffeeCodex.Infrastructure.Persistence;

namespace CoffeeCodex.RecipeListing.Tests;

internal static class RecipeListingTestData
{
    public static readonly Guid EspressoTonicId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    public static readonly Guid IcedMapleLatteId = Guid.Parse("00000000-0000-0000-0000-000000000002");
    public static readonly Guid MatchaCloudId = Guid.Parse("00000000-0000-0000-0000-000000000003");
    public static readonly Guid AffogatoFloatId = Guid.Parse("00000000-0000-0000-0000-000000000004");

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
            AvatarUrl = "https://cdn.example.com/authors/coffee-codex.jpg",
        };

        var authorB = new Author
        {
            Id = Guid.Parse("00000000-0000-0000-0000-0000000000b1"),
            Name = "Guest Barista",
            AvatarUrl = "https://cdn.example.com/authors/guest-barista.jpg",
        };

        var espressoTonic = new Recipe
        {
            Id = EspressoTonicId,
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
            Id = IcedMapleLatteId,
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
            Id = MatchaCloudId,
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
            Id = AffogatoFloatId,
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
                RecipeId = matchaCloud.Id,
                CoffeeDoseInGrams = null,
                CoffeeYieldInGrams = null,
                MilkInMl = 150,
                CupSizeInMl = 300,
                Difficulty = DifficultyLevel.Beginner,
                TimeInMinutes = 4,
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

        var ingredients = new[]
        {
            new RecipeIngredient
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000001"),
                RecipeId = espressoTonic.Id,
                Name = "Espresso",
                QuantityValue = 18m,
                Unit = "g",
                Position = 1,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000002"),
                RecipeId = espressoTonic.Id,
                Name = "Tonic Water",
                QuantityValue = 120m,
                Unit = "ml",
                Position = 2,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000003"),
                RecipeId = icedMapleLatte.Id,
                Name = "Milk",
                QuantityValue = 180m,
                Unit = "ml",
                Position = 1,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000004"),
                RecipeId = icedMapleLatte.Id,
                Name = "Maple Syrup",
                QuantityValue = 15m,
                Unit = "ml",
                Position = 2,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000007"),
                RecipeId = icedMapleLatte.Id,
                Name = "Matcha Foam",
                QuantityValue = 8m,
                Unit = "g",
                Position = 3,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000005"),
                RecipeId = matchaCloud.Id,
                Name = "Matcha Powder",
                QuantityValue = 4m,
                Unit = "g",
                Position = 1,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("20000000-0000-0000-0000-000000000006"),
                RecipeId = affogatoFloat.Id,
                Name = "Vanilla Ice Cream",
                QuantityValue = 80m,
                Unit = "g",
                Position = 1,
            },
        };

        var steps = new[]
        {
            new RecipeStep
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000001"),
                RecipeId = espressoTonic.Id,
                StepNumber = 1,
                Instruction = "Fill glass with ice and tonic.",
            },
            new RecipeStep
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000002"),
                RecipeId = espressoTonic.Id,
                StepNumber = 2,
                Instruction = "Pour espresso on top.",
            },
            new RecipeStep
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000003"),
                RecipeId = icedMapleLatte.Id,
                StepNumber = 1,
                Instruction = "Add syrup and milk to ice.",
            },
            new RecipeStep
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000004"),
                RecipeId = matchaCloud.Id,
                StepNumber = 1,
                Instruction = "Whisk matcha with warm water.",
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
                Caption = null,
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

        var tags = new[]
        {
            new Tag
            {
                Id = Guid.Parse("40000000-0000-0000-0000-000000000001"),
                Name = "citrus",
            },
            new Tag
            {
                Id = Guid.Parse("40000000-0000-0000-0000-000000000002"),
                Name = "iced",
            },
            new Tag
            {
                Id = Guid.Parse("40000000-0000-0000-0000-000000000003"),
                Name = "dessert",
            },
            new Tag
            {
                Id = Guid.Parse("40000000-0000-0000-0000-000000000004"),
                Name = "sparkling",
            },
            new Tag
            {
                Id = Guid.Parse("40000000-0000-0000-0000-000000000005"),
                Name = "matcha",
            },
        };

        var recipeTags = new[]
        {
            new RecipeTag
            {
                RecipeId = espressoTonic.Id,
                TagId = Guid.Parse("40000000-0000-0000-0000-000000000001"),
            },
            new RecipeTag
            {
                RecipeId = espressoTonic.Id,
                TagId = Guid.Parse("40000000-0000-0000-0000-000000000004"),
            },
            new RecipeTag
            {
                RecipeId = icedMapleLatte.Id,
                TagId = Guid.Parse("40000000-0000-0000-0000-000000000002"),
            },
            new RecipeTag
            {
                RecipeId = affogatoFloat.Id,
                TagId = Guid.Parse("40000000-0000-0000-0000-000000000003"),
            },
            new RecipeTag
            {
                RecipeId = matchaCloud.Id,
                TagId = Guid.Parse("40000000-0000-0000-0000-000000000005"),
            },
        };

        await dbContext.Authors.AddRangeAsync(authorA, authorB);
        await dbContext.Tags.AddRangeAsync(tags);
        await dbContext.Recipes.AddRangeAsync(espressoTonic, icedMapleLatte, matchaCloud, affogatoFloat);
        await dbContext.RecipeBrewSpecs.AddRangeAsync(brewSpecs);
        await dbContext.RecipeIngredients.AddRangeAsync(ingredients);
        await dbContext.RecipeSteps.AddRangeAsync(steps);
        await dbContext.RecipeImages.AddRangeAsync(recipeImages);
        await dbContext.RecipeTags.AddRangeAsync(recipeTags);
        await dbContext.SaveChangesAsync();
    }
}
