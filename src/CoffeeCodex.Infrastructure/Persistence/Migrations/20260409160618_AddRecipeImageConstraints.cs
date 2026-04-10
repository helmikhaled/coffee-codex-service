using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeCodex.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRecipeImageConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_recipe_images_recipe_id_position",
                table: "recipe_images");

            migrationBuilder.CreateIndex(
                name: "ix_recipe_images_recipe_id_position",
                table: "recipe_images",
                columns: new[] { "recipe_id", "position" },
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "ck_recipe_images_position_gte_1",
                table: "recipe_images",
                sql: "position >= 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_recipe_images_recipe_id_position",
                table: "recipe_images");

            migrationBuilder.DropCheckConstraint(
                name: "ck_recipe_images_position_gte_1",
                table: "recipe_images");

            migrationBuilder.CreateIndex(
                name: "ix_recipe_images_recipe_id_position",
                table: "recipe_images",
                columns: new[] { "recipe_id", "position" });
        }
    }
}
