using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkdayNotifierBot.Migrations
{
    /// <inheritdoc />
    public partial class Addperiodtype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PeriodType",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PeriodType",
                table: "Users");
        }
    }
}
