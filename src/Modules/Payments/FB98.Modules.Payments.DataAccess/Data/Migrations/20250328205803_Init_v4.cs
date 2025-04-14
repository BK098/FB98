using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FB98.Modules.Payments.DataAccess.Data.Migrations
{
    /// <inheritdoc />
    public partial class Init_v4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpirationDate",
                schema: "PaymentsModule",
                table: "Coupons",
                newName: "StartDate");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "PaymentsModule",
                table: "Coupons",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                schema: "PaymentsModule",
                table: "Coupons",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                schema: "PaymentsModule",
                table: "Coupons");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                schema: "PaymentsModule",
                table: "Coupons",
                newName: "ExpirationDate");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "PaymentsModule",
                table: "Coupons",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);
        }
    }
}
