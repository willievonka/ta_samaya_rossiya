using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "maps",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_analitics = table.Column<bool>(type: "boolean", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    background_image = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_maps", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "region_geometries",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    geometry = table.Column<MultiPolygon>(type: "geometry", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_region_geometries", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "regions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    is_russia = table.Column<bool>(type: "boolean", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_regions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "historical_lines",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    marker_image_path = table.Column<string>(type: "text", nullable: true),
                    line_color = table.Column<string>(type: "text", nullable: false),
                    line_style = table.Column<int>(type: "integer", nullable: false),
                    marker_legend = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    last_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    map_id = table.Column<Guid>(type: "uuid", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "layer_regions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    display_title = table.Column<string>(type: "text", nullable: true),
                    display_title_font_size = table.Column<int>(type: "integer", nullable: false),
                    fill_color = table.Column<string>(type: "text", nullable: false),
                    geometry_id = table.Column<Guid>(type: "uuid", nullable: false),
                    region_id = table.Column<Guid>(type: "uuid", nullable: false),
                    map_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_layer_regions", x => x.id);
                    table.ForeignKey(
                        name: "fk_layer_regions_maps_map_id",
                        column: x => x.map_id,
                        principalTable: "maps",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_layer_regions_region_geometries_geometry_id",
                        column: x => x.geometry_id,
                        principalTable: "region_geometries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_layer_regions_regions_region_id",
                        column: x => x.region_id,
                        principalTable: "regions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "historical_objects",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    coordinates = table.Column<Point>(type: "geometry", nullable: false),
                    number = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    image_path = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    line_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_historical_objects", x => x.id);
                    table.ForeignKey(
                        name: "fk_historical_objects_historical_lines_line_id",
                        column: x => x.line_id,
                        principalTable: "historical_lines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "indicators_regions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    region_id = table.Column<Guid>(type: "uuid", nullable: false),
                    image_path = table.Column<string>(type: "text", nullable: true),
                    excursions = table.Column<int>(type: "integer", nullable: false),
                    partners = table.Column<int>(type: "integer", nullable: false),
                    participants = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_indicators_regions", x => x.id);
                    table.ForeignKey(
                        name: "fk_indicators_regions_layer_regions_region_id",
                        column: x => x.region_id,
                        principalTable: "layer_regions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_historical_lines_map_id",
                table: "historical_lines",
                column: "map_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_historical_objects_line_id",
                table: "historical_objects",
                column: "line_id");

            migrationBuilder.CreateIndex(
                name: "ix_indicators_regions_region_id",
                table: "indicators_regions",
                column: "region_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_layer_regions_geometry_id",
                table: "layer_regions",
                column: "geometry_id");

            migrationBuilder.CreateIndex(
                name: "ix_layer_regions_map_id",
                table: "layer_regions",
                column: "map_id");

            migrationBuilder.CreateIndex(
                name: "ix_layer_regions_region_id",
                table: "layer_regions",
                column: "region_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "historical_objects");

            migrationBuilder.DropTable(
                name: "indicators_regions");

            migrationBuilder.DropTable(
                name: "historical_lines");

            migrationBuilder.DropTable(
                name: "layer_regions");

            migrationBuilder.DropTable(
                name: "maps");

            migrationBuilder.DropTable(
                name: "region_geometries");

            migrationBuilder.DropTable(
                name: "regions");
        }
    }
}
