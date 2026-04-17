using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeCodex.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRecipeSearchIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_tags_name",
                table: "tags");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:pg_trgm", ",,");

            migrationBuilder.CreateIndex(
                name: "ix_tags_name_trgm",
                table: "tags",
                column: "name")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "ix_recipes_title_trgm",
                table: "recipes",
                column: "title")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "ix_recipe_ingredients_name_trgm",
                table: "recipe_ingredients",
                column: "name")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_tags_name_trgm",
                table: "tags");

            migrationBuilder.DropIndex(
                name: "ix_recipes_title_trgm",
                table: "recipes");

            migrationBuilder.DropIndex(
                name: "ix_recipe_ingredients_name_trgm",
                table: "recipe_ingredients");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:pg_trgm", ",,");

            migrationBuilder.CreateIndex(
                name: "ix_tags_name",
                table: "tags",
                column: "name");
        }
    }
}
