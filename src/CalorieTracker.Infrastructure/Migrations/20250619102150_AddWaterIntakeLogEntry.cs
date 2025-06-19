using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalorieTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWaterIntakeLogEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WaterIntakeLogEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AmountMilliliters = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    ConsumedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaterIntakeLogEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WaterIntakeLogEntries_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WaterIntakeLogEntries_ConsumedAt",
                table: "WaterIntakeLogEntries",
                column: "ConsumedAt");

            migrationBuilder.CreateIndex(
                name: "IX_WaterIntakeLogEntries_UserId",
                table: "WaterIntakeLogEntries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WaterIntakeLogEntries_UserId_ConsumedAt",
                table: "WaterIntakeLogEntries",
                columns: new[] { "UserId", "ConsumedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WaterIntakeLogEntries");
        }
    }
}
