using CoffeeCodex.Domain.Recipes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoffeeCodex.Infrastructure.Persistence.Configurations;

internal sealed class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("tags");

        builder.HasKey(tag => tag.Id);

        builder.Property(tag => tag.Id)
            .HasColumnName("id");

        builder.Property(tag => tag.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.HasMany(tag => tag.RecipeTags)
            .WithOne(recipeTag => recipeTag.Tag)
            .HasForeignKey(recipeTag => recipeTag.TagId);
    }
}
