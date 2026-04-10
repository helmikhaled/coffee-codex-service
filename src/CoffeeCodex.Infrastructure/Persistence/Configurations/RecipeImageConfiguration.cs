using CoffeeCodex.Domain.Recipes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoffeeCodex.Infrastructure.Persistence.Configurations;

internal sealed class RecipeImageConfiguration : IEntityTypeConfiguration<RecipeImage>
{
    public void Configure(EntityTypeBuilder<RecipeImage> builder)
    {
        builder.ToTable("recipe_images", tableBuilder =>
        {
            tableBuilder.HasCheckConstraint(
                "ck_recipe_images_position_gte_1",
                $"position >= {RecipeImageConstraints.MinPosition}");
        });

        builder.HasKey(image => image.Id);

        builder.Property(image => image.Id)
            .HasColumnName("id");

        builder.Property(image => image.RecipeId)
            .HasColumnName("recipe_id")
            .IsRequired();

        builder.Property(image => image.BlobUrl)
            .HasColumnName("blob_url")
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(image => image.Caption)
            .HasColumnName("caption")
            .HasMaxLength(500);

        builder.Property(image => image.Position)
            .HasColumnName("position")
            .IsRequired();

        builder.Property(image => image.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.HasIndex(image => new { image.RecipeId, image.Position })
            .IsUnique()
            .HasDatabaseName("ix_recipe_images_recipe_id_position");
    }
}
