using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedonetoonerelationshipbetweenRegionandGeometryremovedthedependencyonLayerRegion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_layer_regions_region_geometries_geometry_id",
                table: "layer_regions");

            migrationBuilder.DropIndex(
                name: "ix_layer_regions_geometry_id",
                table: "layer_regions");

            migrationBuilder.DropColumn(
                name: "geometry_id",
                table: "layer_regions");

            migrationBuilder.AddColumn<Guid>(
                name: "region_id",
                table: "region_geometries",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_region_geometries_region_id",
                table: "region_geometries",
                column: "region_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_region_geometries_regions_region_id",
                table: "region_geometries",
                column: "region_id",
                principalTable: "regions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_region_geometries_regions_region_id",
                table: "region_geometries");

            migrationBuilder.DropIndex(
                name: "ix_region_geometries_region_id",
                table: "region_geometries");

            migrationBuilder.DropColumn(
                name: "region_id",
                table: "region_geometries");

            migrationBuilder.AddColumn<Guid>(
                name: "geometry_id",
                table: "layer_regions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_layer_regions_geometry_id",
                table: "layer_regions",
                column: "geometry_id");

            migrationBuilder.AddForeignKey(
                name: "fk_layer_regions_region_geometries_geometry_id",
                table: "layer_regions",
                column: "geometry_id",
                principalTable: "region_geometries",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
