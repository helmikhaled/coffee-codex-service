using CoffeeCodex.Domain.Recipes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoffeeCodex.Infrastructure.Persistence.Configurations;

internal sealed class RecipeTagConfiguration : IEntityTypeConfiguration<RecipeTag>
{
    public void Configure(EntityTypeBuilder<RecipeTag> builder)
    {
        builder.ToTable("recipe_tags");

        builder.HasKey(recipeTag => new { recipeTag.RecipeId, recipeTag.TagId });

        builder.Property(recipeTag => recipeTag.RecipeId)
            .HasColumnName("recipe_id")
            .IsRequired();

        builder.Property(recipeTag => recipeTag.TagId)
            .HasColumnName("tag_id")
            .IsRequired();

        builder.HasIndex(recipeTag => new { recipeTag.RecipeId, recipeTag.TagId })
            .HasDatabaseName("ix_recipe_tags_recipe_id_tag_id");

        builder.HasIndex(recipeTag => new { recipeTag.TagId, recipeTag.RecipeId })
            .HasDatabaseName("ix_recipe_tags_tag_id_recipe_id");

        builder.HasOne(recipeTag => recipeTag.Recipe)
            .WithMany(recipe => recipe.RecipeTags)
            .HasForeignKey(recipeTag => recipeTag.RecipeId);

        builder.HasOne(recipeTag => recipeTag.Tag)
            .WithMany(tag => tag.RecipeTags)
            .HasForeignKey(recipeTag => recipeTag.TagId);
    }
}
