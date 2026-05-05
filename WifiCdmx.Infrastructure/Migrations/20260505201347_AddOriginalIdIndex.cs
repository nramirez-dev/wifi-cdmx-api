using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WifiCdmx.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOriginalIdIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_WifiPoints_OriginalId",
                table: "WifiPoints",
                column: "OriginalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WifiPoints_OriginalId",
                table: "WifiPoints");
        }
    }
}
