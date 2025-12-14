using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveHistLinerefactorHistObjectaddreferencesatHistObjecttoLayerRegion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_historical_objects_historical_lines_line_id",
                table: "historical_objects");

            migrationBuilder.DropTable(
                name: "historical_lines");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "historical_objects");

            migrationBuilder.RenameColumn(
                name: "number",
                table: "historical_objects",
                newName: "year");

            migrationBuilder.RenameColumn(
                name: "line_id",
                table: "historical_objects",
                newName: "layer_region_id");

            migrationBuilder.RenameIndex(
                name: "ix_historical_objects_line_id",
                table: "historical_objects",
                newName: "ix_historical_objects_layer_region_id");

            migrationBuilder.AddColumn<string>(
                name: "active_layer_regions_color",
                table: "maps",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "historical_object_point_color",
                table: "maps",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "excursion_uri",
                table: "historical_objects",
                type: "text",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_historical_objects_layer_regions_layer_region_id",
                table: "historical_objects",
                column: "layer_region_id",
                principalTable: "layer_regions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_historical_objects_layer_regions_layer_region_id",
                table: "historical_objects");

            migrationBuilder.DropColumn(
                name: "active_layer_regions_color",
                table: "maps");

            migrationBuilder.DropColumn(
                name: "historical_object_point_color",
                table: "maps");

            migrationBuilder.DropColumn(
                name: "excursion_uri",
                table: "historical_objects");

            migrationBuilder.RenameColumn(
                name: "year",
                table: "historical_objects",
                newName: "number");

            migrationBuilder.RenameColumn(
                name: "layer_region_id",
                table: "historical_objects",
                newName: "line_id");

            migrationBuilder.RenameIndex(
                name: "ix_historical_objects_layer_region_id",
                table: "historical_objects",
                newName: "ix_historical_objects_line_id");

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "historical_objects",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "historical_lines",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    map_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    last_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    line_color = table.Column<string>(type: "text", nullable: false),
                    line_style = table.Column<int>(type: "integer", nullable: false),
                    marker_image_path = table.Column<string>(type: "text", nullable: true),
                    marker_legend = table.Column<string>(type: "text", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_historical_lines", x => x.id);
                    table.ForeignKey(
                        name: "fk_historical_lines_maps_map_id",
                        column: x => x.map_id,
                        principalTable: "maps",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_historical_lines_map_id",
                table: "historical_lines",
                column: "map_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_historical_objects_historical_lines_line_id",
                table: "historical_objects",
                column: "line_id",
                principalTable: "historical_lines",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
