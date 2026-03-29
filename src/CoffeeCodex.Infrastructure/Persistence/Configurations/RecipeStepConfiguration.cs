using CoffeeCodex.Domain.Recipes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoffeeCodex.Infrastructure.Persistence.Configurations;

internal sealed class RecipeStepConfiguration : IEntityTypeConfiguration<RecipeStep>
{
    public void Configure(EntityTypeBuilder<RecipeStep> builder)
    {
        builder.ToTable("recipe_steps");

        builder.HasKey(step => step.Id);

        builder.Property(step => step.Id)
            .HasColumnName("id");

        builder.Property(step => step.RecipeId)
            .HasColumnName("recipe_id")
            .IsRequired();

        builder.Property(step => step.StepNumber)
            .HasColumnName("step_number")
            .IsRequired();

        builder.Property(step => step.Instruction)
            .HasColumnName("instruction")
            .HasMaxLength(2000)
            .IsRequired();

        builder.HasIndex(step => new { step.RecipeId, step.StepNumber })
            .HasDatabaseName("ix_recipe_steps_recipe_id_step_number");

        builder.HasOne(step => step.Recipe)
            .WithMany(recipe => recipe.Steps)
            .HasForeignKey(step => step.RecipeId);
    }
}
