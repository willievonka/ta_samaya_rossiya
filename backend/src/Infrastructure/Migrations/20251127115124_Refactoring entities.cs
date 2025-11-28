using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Refactoringentities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "type",
                table: "regions");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "region_geometries");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "region_geometries");

            migrationBuilder.DropColumn(
                name: "display_title",
                table: "layer_regions");

            migrationBuilder.DropColumn(
                name: "display_title_font_size",
                table: "layer_regions");

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "maps",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "maps",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_at",
                table: "maps");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "maps");

            migrationBuilder.AddColumn<int>(
                name: "type",
                table: "regions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "region_geometries",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "region_geometries",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "display_title",
                table: "layer_regions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "display_title_font_size",
                table: "layer_regions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
