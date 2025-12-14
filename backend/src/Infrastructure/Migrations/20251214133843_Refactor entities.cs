using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Refactorentities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "excursion_uri",
                table: "historical_objects",
                newName: "excursion_url");

            migrationBuilder.AddColumn<string>(
                name: "info",
                table: "maps",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "info",
                table: "maps");

            migrationBuilder.RenameColumn(
                name: "excursion_url",
                table: "historical_objects",
                newName: "excursion_uri");
        }
    }
}
