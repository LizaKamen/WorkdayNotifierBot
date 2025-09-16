using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkdayNotifierBot.Migrations
{
    /// <inheritdoc />
    public partial class AddIsWorkFinishedForTodayproperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsWorkFinishedForToday",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsWorkFinishedForToday",
                table: "Users");
        }
    }
}
