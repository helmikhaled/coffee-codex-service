using CoffeeCodex.Domain.Recipes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoffeeCodex.Infrastructure.Persistence.Configurations;

internal sealed class RecipeIngredientConfiguration : IEntityTypeConfiguration<RecipeIngredient>
{
    public void Configure(EntityTypeBuilder<RecipeIngredient> builder)
    {
        builder.ToTable("recipe_ingredients");

        builder.HasKey(ingredient => ingredient.Id);

        builder.Property(ingredient => ingredient.Id)
            .HasColumnName("id");

        builder.Property(ingredient => ingredient.RecipeId)
            .HasColumnName("recipe_id")
            .IsRequired();

        builder.Property(ingredient => ingredient.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(ingredient => ingredient.QuantityValue)
            .HasColumnName("quantity_value")
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(ingredient => ingredient.Unit)
            .HasColumnName("unit")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(ingredient => ingredient.Position)
            .HasColumnName("position")
            .IsRequired();

        builder.HasIndex(ingredient => new { ingredient.RecipeId, ingredient.Position })
            .HasDatabaseName("ix_recipe_ingredients_recipe_id_position");

        builder.HasOne(ingredient => ingredient.Recipe)
            .WithMany(recipe => recipe.Ingredients)
            .HasForeignKey(ingredient => ingredient.RecipeId);
    }
}
