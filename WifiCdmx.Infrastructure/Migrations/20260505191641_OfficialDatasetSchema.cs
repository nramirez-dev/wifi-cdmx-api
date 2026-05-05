using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WifiCdmx.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OfficialDatasetSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessPointCount",
                table: "WifiPoints");

            migrationBuilder.DropColumn(
                name: "Neighborhood",
                table: "WifiPoints");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "WifiPoints",
                newName: "OriginalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OriginalId",
                table: "WifiPoints",
                newName: "Name");

            migrationBuilder.AddColumn<int>(
                name: "AccessPointCount",
                table: "WifiPoints",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Neighborhood",
                table: "WifiPoints",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
