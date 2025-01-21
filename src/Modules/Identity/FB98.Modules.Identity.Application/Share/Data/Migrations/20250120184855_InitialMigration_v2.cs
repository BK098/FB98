using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FB98.Modules.Identity.Application.Share.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration_v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRevoked",
                schema: "IdentityModule",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "RevokedAt",
                schema: "IdentityModule",
                table: "AspNetUsers",
                type: "timestamp without time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRevoked",
                schema: "IdentityModule",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RevokedAt",
                schema: "IdentityModule",
                table: "AspNetUsers");
        }
    }
}
