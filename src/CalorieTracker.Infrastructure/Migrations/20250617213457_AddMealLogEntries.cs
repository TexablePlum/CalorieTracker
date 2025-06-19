using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalorieTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMealLogEntries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MealLogEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RecipeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Quantity = table.Column<float>(type: "real", nullable: false),
                    MealType = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ConsumedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CalculatedCalories = table.Column<float>(type: "real", nullable: false),
                    CalculatedProtein = table.Column<float>(type: "real", nullable: false),
                    CalculatedFat = table.Column<float>(type: "real", nullable: false),
                    CalculatedCarbohydrates = table.Column<float>(type: "real", nullable: false),
                    CalculatedFiber = table.Column<float>(type: "real", nullable: true),
                    CalculatedSugar = table.Column<float>(type: "real", nullable: true),
                    CalculatedSodium = table.Column<float>(type: "real", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealLogEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MealLogEntries_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MealLogEntries_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MealLogEntries_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MealLogEntries_ConsumedAt",
                table: "MealLogEntries",
                column: "ConsumedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MealLogEntries_MealType",
                table: "MealLogEntries",
                column: "MealType");

            migrationBuilder.CreateIndex(
                name: "IX_MealLogEntries_ProductId",
                table: "MealLogEntries",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_MealLogEntries_RecipeId",
                table: "MealLogEntries",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_MealLogEntries_UserId",
                table: "MealLogEntries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MealLogEntries_UserId_ConsumedAt",
                table: "MealLogEntries",
                columns: new[] { "UserId", "ConsumedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MealLogEntries");
        }
    }
}
