using CoffeeCodex.Domain.Recipes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoffeeCodex.Infrastructure.Persistence.Configurations;

internal sealed class RecipeBrewSpecsConfiguration : IEntityTypeConfiguration<RecipeBrewSpecs>
{
    public void Configure(EntityTypeBuilder<RecipeBrewSpecs> builder)
    {
        builder.ToTable("recipe_brew_specs");

        builder.HasKey(brewSpecs => brewSpecs.RecipeId);

        builder.Property(brewSpecs => brewSpecs.RecipeId)
            .HasColumnName("recipe_id");

        builder.Property(brewSpecs => brewSpecs.CoffeeDoseInGrams)
            .HasColumnName("coffee_dose_in_grams");

        builder.Property(brewSpecs => brewSpecs.CoffeeYieldInGrams)
            .HasColumnName("coffee_yield_in_grams");

        builder.Property(brewSpecs => brewSpecs.MilkInMl)
            .HasColumnName("milk_in_ml");

        builder.Property(brewSpecs => brewSpecs.CupSizeInMl)
            .HasColumnName("cup_size_in_ml");

        builder.Property(brewSpecs => brewSpecs.Difficulty)
            .HasColumnName("difficulty")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(brewSpecs => brewSpecs.TimeInMinutes)
            .HasColumnName("time_in_minutes")
            .IsRequired();
    }
}
