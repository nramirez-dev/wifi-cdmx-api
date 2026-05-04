using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WifiCdmx.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGuidIdAndNameField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // PostgreSQL cannot auto-cast varchar to uuid — explicit USING clause required
            migrationBuilder.Sql(@"ALTER TABLE ""WifiPoints"" ALTER COLUMN ""Id"" TYPE uuid USING ""Id""::uuid");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "WifiPoints",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "WifiPoints");

            migrationBuilder.Sql(@"ALTER TABLE ""WifiPoints"" ALTER COLUMN ""Id"" TYPE character varying(200) USING ""Id""::text");
        }
    }
}
