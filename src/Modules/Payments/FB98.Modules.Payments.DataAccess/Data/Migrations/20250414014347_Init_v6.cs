using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FB98.Modules.Payments.DataAccess.Data.Migrations
{
    /// <inheritdoc />
    public partial class Init_v6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "VNPayTransactionId",
                schema: "PaymentsModule",
                table: "PaymentTransactions",
                type: "character varying(36)",
                maxLength: 36,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                schema: "PaymentsModule",
                table: "PaymentTransactions",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                schema: "PaymentsModule",
                table: "PaymentTransactions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "CouponCode",
                schema: "PaymentsModule",
                table: "PaymentTransactions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SubAmount",
                schema: "PaymentsModule",
                table: "PaymentTransactions",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "PaymentsModule",
                table: "PaymentStatuses",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "PaymentsModule",
                table: "PaymentMethods",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "PaymentsModule",
                table: "Coupons",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<int>(
                name: "SoftUsageCount",
                schema: "PaymentsModule",
                table: "Coupons",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "AppliedAmount",
                schema: "PaymentsModule",
                table: "CouponApplications",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "AppliedAt",
                schema: "PaymentsModule",
                table: "CouponApplications",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CouponCode",
                schema: "PaymentsModule",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "SubAmount",
                schema: "PaymentsModule",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "PaymentsModule",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "SoftUsageCount",
                schema: "PaymentsModule",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "AppliedAmount",
                schema: "PaymentsModule",
                table: "CouponApplications");

            migrationBuilder.DropColumn(
                name: "AppliedAt",
                schema: "PaymentsModule",
                table: "CouponApplications");

            migrationBuilder.AlterColumn<string>(
                name: "VNPayTransactionId",
                schema: "PaymentsModule",
                table: "PaymentTransactions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(36)",
                oldMaxLength: 36,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                schema: "PaymentsModule",
                table: "PaymentTransactions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                schema: "PaymentsModule",
                table: "PaymentTransactions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "PaymentsModule",
                table: "PaymentStatuses",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "PaymentsModule",
                table: "PaymentMethods",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);
        }
    }
}
