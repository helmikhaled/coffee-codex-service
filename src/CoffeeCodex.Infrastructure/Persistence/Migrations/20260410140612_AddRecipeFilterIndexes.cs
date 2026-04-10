using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeCodex.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRecipeFilterIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_recipe_tags_tag_id",
                table: "recipe_tags");

            migrationBuilder.CreateIndex(
                name: "ix_tags_name",
                table: "tags",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_recipes_category",
                table: "recipes",
                column: "category");

            migrationBuilder.CreateIndex(
                name: "ix_recipe_tags_tag_id_recipe_id",
                table: "recipe_tags",
                columns: new[] { "tag_id", "recipe_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_tags_name",
                table: "tags");

            migrationBuilder.DropIndex(
                name: "ix_recipes_category",
                table: "recipes");

            migrationBuilder.DropIndex(
                name: "ix_recipe_tags_tag_id_recipe_id",
                table: "recipe_tags");

            migrationBuilder.CreateIndex(
                name: "IX_recipe_tags_tag_id",
                table: "recipe_tags",
                column: "tag_id");
        }
    }
}
