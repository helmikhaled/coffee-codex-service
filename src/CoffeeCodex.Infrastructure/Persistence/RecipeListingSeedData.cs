using CoffeeCodex.Domain.Recipes;
using Microsoft.EntityFrameworkCore;

namespace CoffeeCodex.Infrastructure.Persistence;

internal static class RecipeListingSeedData
{
    public static async Task SeedAsync(CoffeeCodexDbContext dbContext)
    {
        if (await dbContext.Recipes.AnyAsync())
        {
            return;
        }

        var now = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var authorA = new Author
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Name = "Coffee Codex",
            AvatarUrl = "https://cdn.coffeecodex.dev/authors/coffee-codex.jpg",
        };

        var authorB = new Author
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Name = "Guest Barista",
            AvatarUrl = "https://cdn.coffeecodex.dev/authors/guest-barista.jpg",
        };

        var espressoTonic = new Recipe
        {
            Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"),
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
            Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"),
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
            Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3"),
            Title = "Matcha Cloud",
            Description = "A soft iced matcha drink with no espresso.",
            Category = RecipeCategory.Modern,
            Slug = "matcha-cloud",
            AuthorId = authorA.Id,
            BrewCount = 43,
            DisplayOrder = 3,
            CreatedAt = now,
            UpdatedAt = now,
        };

        var affogatoFloat = new Recipe
        {
            Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4"),
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

        var orangeAmericano = new Recipe
        {
            Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa5"),
            Title = "Orange Americano",
            Description = "Espresso with fresh orange juice over ice.",
            Category = RecipeCategory.Citrus,
            Slug = "orange-americano",
            AuthorId = authorA.Id,
            BrewCount = 0,
            DisplayOrder = 5,
            CreatedAt = now,
            UpdatedAt = now,
        };

        var dirtyLatte = new Recipe
        {
            Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa6"),
            Title = "Dirty Latte",
            Description = "Cold milk with a shot of espresso layered on top.",
            Category = RecipeCategory.Iced,
            Slug = "dirty-latte",
            AuthorId = authorB.Id,
            BrewCount = 0,
            DisplayOrder = 6,
            CreatedAt = now,
            UpdatedAt = now,
        };

        var espressoShakerato = new Recipe
        {
            Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa7"),
            Title = "Espresso Shakerato",
            Description = "Shaken espresso with ice for a smooth, frothy texture.",
            Category = RecipeCategory.Modern,
            Slug = "espresso-shakerato",
            AuthorId = authorA.Id,
            BrewCount = 0,
            DisplayOrder = 7,
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
                CoffeeYieldInGrams = 36m,
                CupSizeInMl = 220,
                Difficulty = DifficultyLevel.Advanced,
                TimeInMinutes = 7,
            },
            new RecipeBrewSpecs
            {
                RecipeId = orangeAmericano.Id,
                CoffeeDoseInGrams = 18m,
                CoffeeYieldInGrams = 36m,
                CupSizeInMl = 250,
                Difficulty = DifficultyLevel.Beginner,
                TimeInMinutes = 5,
            },
            new RecipeBrewSpecs
            {
                RecipeId = dirtyLatte.Id,
                CoffeeDoseInGrams = 18m,
                CoffeeYieldInGrams = 36m,
                MilkInMl = 180,
                CupSizeInMl = 250,
                Difficulty = DifficultyLevel.Beginner,
                TimeInMinutes = 4,
            },
            new RecipeBrewSpecs
            {
                RecipeId = espressoShakerato.Id,
                CoffeeDoseInGrams = 18m,
                CoffeeYieldInGrams = 36m,
                CupSizeInMl = 180,
                Difficulty = DifficultyLevel.Intermediate,
                TimeInMinutes = 3,
            },
        };

        var ingredients = new[]
        {
            new RecipeIngredient
            {
                Id = Guid.Parse("41000000-0000-0000-0000-000000000001"),
                RecipeId = espressoTonic.Id,
                Name = "Espresso",
                QuantityValue = 18m,
                Unit = "g",
                Position = 1,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("41000000-0000-0000-0000-000000000002"),
                RecipeId = espressoTonic.Id,
                Name = "Tonic Water",
                QuantityValue = 120m,
                Unit = "ml",
                Position = 2,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("41000000-0000-0000-0000-000000000003"),
                RecipeId = espressoTonic.Id,
                Name = "Orange Peel",
                QuantityValue = 1m,
                Unit = "piece",
                Position = 3,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("41000000-0000-0000-0000-000000000004"),
                RecipeId = icedMapleLatte.Id,
                Name = "Espresso",
                QuantityValue = 18m,
                Unit = "g",
                Position = 1,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("41000000-0000-0000-0000-000000000005"),
                RecipeId = icedMapleLatte.Id,
                Name = "Milk",
                QuantityValue = 180m,
                Unit = "ml",
                Position = 2,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("41000000-0000-0000-0000-000000000006"),
                RecipeId = icedMapleLatte.Id,
                Name = "Maple Syrup",
                QuantityValue = 15m,
                Unit = "ml",
                Position = 3,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("41000000-0000-0000-0000-000000000007"),
                RecipeId = matchaCloud.Id,
                Name = "Matcha Powder",
                QuantityValue = 4m,
                Unit = "g",
                Position = 1,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("41000000-0000-0000-0000-000000000008"),
                RecipeId = matchaCloud.Id,
                Name = "Milk",
                QuantityValue = 150m,
                Unit = "ml",
                Position = 2,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("41000000-0000-0000-0000-000000000009"),
                RecipeId = matchaCloud.Id,
                Name = "Honey",
                QuantityValue = 10m,
                Unit = "ml",
                Position = 3,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("41000000-0000-0000-0000-000000000010"),
                RecipeId = affogatoFloat.Id,
                Name = "Espresso",
                QuantityValue = 18m,
                Unit = "g",
                Position = 1,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("41000000-0000-0000-0000-000000000011"),
                RecipeId = affogatoFloat.Id,
                Name = "Vanilla Ice Cream",
                QuantityValue = 80m,
                Unit = "g",
                Position = 2,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("41000000-0000-0000-0000-000000000012"),
                RecipeId = orangeAmericano.Id,
                Name = "Espresso",
                QuantityValue = 18m,
                Unit = "g",
                Position = 1,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("41000000-0000-0000-0000-000000000013"),
                RecipeId = orangeAmericano.Id,
                Name = "Orange Juice",
                QuantityValue = 90m,
                Unit = "ml",
                Position = 2,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("41000000-0000-0000-0000-000000000014"),
                RecipeId = dirtyLatte.Id,
                Name = "Espresso",
                QuantityValue = 18m,
                Unit = "g",
                Position = 1,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("41000000-0000-0000-0000-000000000015"),
                RecipeId = dirtyLatte.Id,
                Name = "Cold Milk",
                QuantityValue = 180m,
                Unit = "ml",
                Position = 2,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("41000000-0000-0000-0000-000000000016"),
                RecipeId = espressoShakerato.Id,
                Name = "Espresso",
                QuantityValue = 18m,
                Unit = "g",
                Position = 1,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("41000000-0000-0000-0000-000000000017"),
                RecipeId = espressoShakerato.Id,
                Name = "Simple Syrup",
                QuantityValue = 10m,
                Unit = "ml",
                Position = 2,
            },
            new RecipeIngredient
            {
                Id = Guid.Parse("41000000-0000-0000-0000-000000000018"),
                RecipeId = espressoShakerato.Id,
                Name = "Ice",
                QuantityValue = 120m,
                Unit = "g",
                Position = 3,
            },
        };

        var steps = new[]
        {
            new RecipeStep
            {
                Id = Guid.Parse("42000000-0000-0000-0000-000000000001"),
                RecipeId = espressoTonic.Id,
                StepNumber = 1,
                Instruction = "Fill a tall glass with ice and tonic water.",
            },
            new RecipeStep
            {
                Id = Guid.Parse("42000000-0000-0000-0000-000000000002"),
                RecipeId = espressoTonic.Id,
                StepNumber = 2,
                Instruction = "Pull a double espresso shot.",
            },
            new RecipeStep
            {
                Id = Guid.Parse("42000000-0000-0000-0000-000000000003"),
                RecipeId = espressoTonic.Id,
                StepNumber = 3,
                Instruction = "Gently pour espresso over tonic and garnish with orange peel.",
            },
            new RecipeStep
            {
                Id = Guid.Parse("42000000-0000-0000-0000-000000000004"),
                RecipeId = icedMapleLatte.Id,
                StepNumber = 1,
                Instruction = "Add maple syrup to a glass of ice.",
            },
            new RecipeStep
            {
                Id = Guid.Parse("42000000-0000-0000-0000-000000000005"),
                RecipeId = icedMapleLatte.Id,
                StepNumber = 2,
                Instruction = "Pour in cold milk and stir.",
            },
            new RecipeStep
            {
                Id = Guid.Parse("42000000-0000-0000-0000-000000000006"),
                RecipeId = icedMapleLatte.Id,
                StepNumber = 3,
                Instruction = "Top with freshly brewed espresso.",
            },
            new RecipeStep
            {
                Id = Guid.Parse("42000000-0000-0000-0000-000000000007"),
                RecipeId = matchaCloud.Id,
                StepNumber = 1,
                Instruction = "Whisk matcha powder with a small amount of warm water.",
            },
            new RecipeStep
            {
                Id = Guid.Parse("42000000-0000-0000-0000-000000000008"),
                RecipeId = matchaCloud.Id,
                StepNumber = 2,
                Instruction = "Add honey and whisk until smooth.",
            },
            new RecipeStep
            {
                Id = Guid.Parse("42000000-0000-0000-0000-000000000009"),
                RecipeId = matchaCloud.Id,
                StepNumber = 3,
                Instruction = "Pour over iced milk.",
            },
            new RecipeStep
            {
                Id = Guid.Parse("42000000-0000-0000-0000-000000000010"),
                RecipeId = affogatoFloat.Id,
                StepNumber = 1,
                Instruction = "Place vanilla ice cream in a chilled glass.",
            },
            new RecipeStep
            {
                Id = Guid.Parse("42000000-0000-0000-0000-000000000011"),
                RecipeId = affogatoFloat.Id,
                StepNumber = 2,
                Instruction = "Pull a fresh espresso shot.",
            },
            new RecipeStep
            {
                Id = Guid.Parse("42000000-0000-0000-0000-000000000012"),
                RecipeId = affogatoFloat.Id,
                StepNumber = 3,
                Instruction = "Pour espresso over the ice cream and serve immediately.",
            },
            new RecipeStep
            {
                Id = Guid.Parse("42000000-0000-0000-0000-000000000013"),
                RecipeId = orangeAmericano.Id,
                StepNumber = 1,
                Instruction = "Fill a glass with ice and orange juice.",
            },
            new RecipeStep
            {
                Id = Guid.Parse("42000000-0000-0000-0000-000000000014"),
                RecipeId = orangeAmericano.Id,
                StepNumber = 2,
                Instruction = "Add espresso slowly to create a layered effect.",
            },
            new RecipeStep
            {
                Id = Guid.Parse("42000000-0000-0000-0000-000000000015"),
                RecipeId = dirtyLatte.Id,
                StepNumber = 1,
                Instruction = "Pour cold milk over ice in a clear glass.",
            },
            new RecipeStep
            {
                Id = Guid.Parse("42000000-0000-0000-0000-000000000016"),
                RecipeId = dirtyLatte.Id,
                StepNumber = 2,
                Instruction = "Top with espresso to create distinct layers.",
            },
            new RecipeStep
            {
                Id = Guid.Parse("42000000-0000-0000-0000-000000000017"),
                RecipeId = espressoShakerato.Id,
                StepNumber = 1,
                Instruction = "Add espresso, syrup, and ice into a shaker.",
            },
            new RecipeStep
            {
                Id = Guid.Parse("42000000-0000-0000-0000-000000000018"),
                RecipeId = espressoShakerato.Id,
                StepNumber = 2,
                Instruction = "Shake vigorously for 15 seconds.",
            },
            new RecipeStep
            {
                Id = Guid.Parse("42000000-0000-0000-0000-000000000019"),
                RecipeId = espressoShakerato.Id,
                StepNumber = 3,
                Instruction = "Strain into a chilled glass.",
            },
        };

        var recipeImages = new[]
        {
            new RecipeImage
            {
                Id = Guid.Parse("f1c9a9d2-3b2c-4f2e-9c3a-1a7e8d6b1a01"),
                RecipeId = espressoTonic.Id,
                BlobUrl = "https://plus.unsplash.com/premium_photo-1671559020928-dde18021036f?q=80&w=687&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
                Caption = "Espresso tonic",
                Position = 1,
                CreatedAt = now,
            },
            new RecipeImage
            {
                Id = Guid.Parse("b7a0f4c1-9c45-4a7c-bb2d-5b3e8a1c2f02"),
                RecipeId = espressoTonic.Id,
                BlobUrl = "https://images.unsplash.com/photo-1609050471053-8636409f9f5b?q=80&w=774&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
                Caption = "Espresso tonic detail",
                Position = 2,
                CreatedAt = now,
            },
            new RecipeImage
            {
                Id = Guid.Parse("a2e4c7b9-6c1d-4d4e-8c52-9d1e3b7a4f03"),
                RecipeId = icedMapleLatte.Id,
                BlobUrl = "https://images.unsplash.com/photo-1461023058943-07fcbe16d735?q=80&w=1738&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
                Caption = "Iced latte",
                Position = 1,
                CreatedAt = now,
            },
            new RecipeImage
            {
                Id = Guid.Parse("c3d8b1a2-2e54-4c8e-9d6b-0e1a2f3b4c04"),
                RecipeId = matchaCloud.Id,
                BlobUrl = "https://images.unsplash.com/photo-1695191499096-8a7ffe8d5b8e?q=80&w=928&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
                Caption = "Matcha drink",
                Position = 1,
                CreatedAt = now,
            },
            new RecipeImage
            {
                Id = Guid.Parse("d4a7f6c3-5b12-4f8b-9e1c-7c3a2b1d5e05"),
                RecipeId = affogatoFloat.Id,
                BlobUrl = "https://images.unsplash.com/photo-1594631661960-34762327295a?q=80&w=772&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
                Caption = "Affogato",
                Position = 1,
                CreatedAt = now,
            },
            new RecipeImage
            {
                Id = Guid.Parse("e5b2c1d4-8f6a-4c9e-a7d3-2f1b6c4e7a06"),
                RecipeId = affogatoFloat.Id,
                BlobUrl = "https://images.unsplash.com/photo-1594631661960-34762327295a?q=80&w=772&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
                Caption = "Affogato detail",
                Position = 2,
                CreatedAt = now,
            },
            new RecipeImage
            {
                Id = Guid.Parse("f6c3a2b1-7d4e-4e9f-b8c1-3a2d5e6f7b07"),
                RecipeId = orangeAmericano.Id,
                BlobUrl = "https://plus.unsplash.com/premium_photo-1671559020931-c103ecf61a13?q=80&w=774&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
                Caption = null,
                Position = 1,
                CreatedAt = now,
            },
            new RecipeImage
            {
                Id = Guid.Parse("b8e5f6a7-2c3d-4b9f-a1c2-5d6e7f8a9b09"),
                RecipeId = espressoShakerato.Id,
                BlobUrl = "https://images.unsplash.com/photo-1607687331899-4f157b139583?q=80&w=1740&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
                Caption = "Espresso shakerato",
                Position = 1,
                CreatedAt = now,
            },
        };

        var tags = new[]
        {
            new Tag
            {
                Id = Guid.Parse("51000000-0000-0000-0000-000000000001"),
                Name = "citrus",
            },
            new Tag
            {
                Id = Guid.Parse("51000000-0000-0000-0000-000000000002"),
                Name = "iced",
            },
            new Tag
            {
                Id = Guid.Parse("51000000-0000-0000-0000-000000000003"),
                Name = "sweet",
            },
            new Tag
            {
                Id = Guid.Parse("51000000-0000-0000-0000-000000000004"),
                Name = "matcha",
            },
            new Tag
            {
                Id = Guid.Parse("51000000-0000-0000-0000-000000000005"),
                Name = "dessert",
            },
            new Tag
            {
                Id = Guid.Parse("51000000-0000-0000-0000-000000000006"),
                Name = "sparkling",
            },
        };

        var recipeTags = new[]
        {
            new RecipeTag
            {
                RecipeId = espressoTonic.Id,
                TagId = Guid.Parse("51000000-0000-0000-0000-000000000001"),
            },
            new RecipeTag
            {
                RecipeId = espressoTonic.Id,
                TagId = Guid.Parse("51000000-0000-0000-0000-000000000006"),
            },
            new RecipeTag
            {
                RecipeId = icedMapleLatte.Id,
                TagId = Guid.Parse("51000000-0000-0000-0000-000000000002"),
            },
            new RecipeTag
            {
                RecipeId = icedMapleLatte.Id,
                TagId = Guid.Parse("51000000-0000-0000-0000-000000000003"),
            },
            new RecipeTag
            {
                RecipeId = matchaCloud.Id,
                TagId = Guid.Parse("51000000-0000-0000-0000-000000000002"),
            },
            new RecipeTag
            {
                RecipeId = matchaCloud.Id,
                TagId = Guid.Parse("51000000-0000-0000-0000-000000000004"),
            },
            new RecipeTag
            {
                RecipeId = affogatoFloat.Id,
                TagId = Guid.Parse("51000000-0000-0000-0000-000000000005"),
            },
            new RecipeTag
            {
                RecipeId = orangeAmericano.Id,
                TagId = Guid.Parse("51000000-0000-0000-0000-000000000001"),
            },
            new RecipeTag
            {
                RecipeId = orangeAmericano.Id,
                TagId = Guid.Parse("51000000-0000-0000-0000-000000000002"),
            },
            new RecipeTag
            {
                RecipeId = espressoShakerato.Id,
                TagId = Guid.Parse("51000000-0000-0000-0000-000000000002"),
            },
        };

        await dbContext.Authors.AddRangeAsync(authorA, authorB);
        await dbContext.Tags.AddRangeAsync(tags);
        await dbContext.Recipes.AddRangeAsync(
            espressoTonic,
            icedMapleLatte,
            matchaCloud,
            affogatoFloat,
            orangeAmericano,
            dirtyLatte,
            espressoShakerato);
        await dbContext.RecipeBrewSpecs.AddRangeAsync(brewSpecs);
        await dbContext.RecipeIngredients.AddRangeAsync(ingredients);
        await dbContext.RecipeSteps.AddRangeAsync(steps);
        await dbContext.RecipeImages.AddRangeAsync(recipeImages);
        await dbContext.RecipeTags.AddRangeAsync(recipeTags);
        await dbContext.SaveChangesAsync();
    }
}
