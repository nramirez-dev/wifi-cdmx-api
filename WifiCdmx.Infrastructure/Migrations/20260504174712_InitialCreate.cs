using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WifiCdmx.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WifiPoints",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Neighborhood = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Borough = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    AccessPointCount = table.Column<int>(type: "integer", nullable: false),
                    Program = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WifiPoints", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WifiPoints_Borough",
                table: "WifiPoints",
                column: "Borough");

            migrationBuilder.CreateIndex(
                name: "IX_WifiPoints_Program",
                table: "WifiPoints",
                column: "Program");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WifiPoints");
        }
    }
}
