using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalorieTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTargetWeightAndMealPlanToUserProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MealPlan",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<float>(
                name: "TargetWeightKg",
                table: "UserProfiles",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "WeeklyGoalChangeKg",
                table: "UserProfiles",
                type: "real",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MealPlan",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "TargetWeightKg",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "WeeklyGoalChangeKg",
                table: "UserProfiles");
        }
    }
}
