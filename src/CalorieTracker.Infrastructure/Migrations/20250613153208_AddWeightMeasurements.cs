using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalorieTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWeightMeasurements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WeightMeasurements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MeasurementDate = table.Column<DateOnly>(type: "date", nullable: false),
                    WeightKg = table.Column<float>(type: "real", nullable: false),
                    BMI = table.Column<float>(type: "real", nullable: false),
                    WeightChangeKg = table.Column<float>(type: "real", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeightMeasurements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeightMeasurements_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WeightMeasurements_MeasurementDate",
                table: "WeightMeasurements",
                column: "MeasurementDate");

            migrationBuilder.CreateIndex(
                name: "IX_WeightMeasurements_UserId",
                table: "WeightMeasurements",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WeightMeasurements_UserId_MeasurementDate",
                table: "WeightMeasurements",
                columns: new[] { "UserId", "MeasurementDate" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WeightMeasurements");
        }
    }
}
