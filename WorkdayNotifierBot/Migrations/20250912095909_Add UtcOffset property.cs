using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkdayNotifierBot.Migrations
{
    /// <inheritdoc />
    public partial class AddUtcOffsetproperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UtcOffset",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UtcOffset",
                table: "Users");
        }
    }
}
