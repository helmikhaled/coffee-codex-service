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
        };

        var authorB = new Author
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Name = "Guest Barista",
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
                BlobUrl = "https://plus.unsplash.com/premium_photo-1671559020928-dde18021036f?q=80&w=687&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
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
                Caption = "Orange espresso",
                Position = 1,
                CreatedAt = now,
            },
            new RecipeImage
            {
                Id = Guid.Parse("a7d4e5f6-1b2c-4a8f-9c3d-4e5f6a7b8c08"),
                RecipeId = dirtyLatte.Id,
                BlobUrl = "https://images.unsplash.com/photo-1598778124054-f9548b4d5e4c?q=80&w=1178&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
                Caption = "Dirty latte layered",
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

        await dbContext.Authors.AddRangeAsync(authorA, authorB);

        await dbContext.Recipes.AddRangeAsync(
            espressoTonic,
            icedMapleLatte,
            matchaCloud,
            affogatoFloat,
            orangeAmericano,
            dirtyLatte,
            espressoShakerato
        );

        await dbContext.RecipeBrewSpecs.AddRangeAsync(brewSpecs);
        await dbContext.RecipeImages.AddRangeAsync(recipeImages);

        await dbContext.SaveChangesAsync();
    }
}