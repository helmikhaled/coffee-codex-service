using CoffeeCodex.Domain.Recipes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoffeeCodex.Infrastructure.Persistence.Configurations;

internal sealed class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        builder.ToTable("recipes");

        builder.HasKey(recipe => recipe.Id);

        builder.Property(recipe => recipe.Id)
            .HasColumnName("id");

        builder.Property(recipe => recipe.Title)
            .HasColumnName("title")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(recipe => recipe.Description)
            .HasColumnName("description")
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(recipe => recipe.Category)
            .HasColumnName("category")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(recipe => recipe.Slug)
            .HasColumnName("slug")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(recipe => recipe.AuthorId)
            .HasColumnName("author_id")
            .IsRequired();

        builder.Property(recipe => recipe.BrewCount)
            .HasColumnName("brew_count")
            .IsRequired();

        builder.Property(recipe => recipe.DisplayOrder)
            .HasColumnName("display_order")
            .IsRequired();

        builder.Property(recipe => recipe.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(recipe => recipe.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.HasIndex(recipe => recipe.DisplayOrder)
            .HasDatabaseName("ix_recipes_display_order");

        builder.HasOne(recipe => recipe.Author)
            .WithMany(author => author.Recipes)
            .HasForeignKey(recipe => recipe.AuthorId);

        builder.HasOne(recipe => recipe.BrewSpecs)
            .WithOne(brewSpecs => brewSpecs.Recipe)
            .HasForeignKey<RecipeBrewSpecs>(brewSpecs => brewSpecs.RecipeId);

        builder.HasMany(recipe => recipe.Images)
            .WithOne(image => image.Recipe)
            .HasForeignKey(image => image.RecipeId);

        builder.HasMany(recipe => recipe.Ingredients)
            .WithOne(ingredient => ingredient.Recipe)
            .HasForeignKey(ingredient => ingredient.RecipeId);

        builder.HasMany(recipe => recipe.Steps)
            .WithOne(step => step.Recipe)
            .HasForeignKey(step => step.RecipeId);

        builder.HasMany(recipe => recipe.RecipeTags)
            .WithOne(recipeTag => recipeTag.Recipe)
            .HasForeignKey(recipeTag => recipeTag.RecipeId);
    }
}
