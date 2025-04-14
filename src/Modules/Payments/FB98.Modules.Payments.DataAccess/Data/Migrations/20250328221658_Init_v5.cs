using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FB98.Modules.Payments.DataAccess.Data.Migrations
{
    /// <inheritdoc />
    public partial class Init_v5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MaxDiscountAmount",
                schema: "PaymentsModule",
                table: "Coupons",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinPaymentAmount",
                schema: "PaymentsModule",
                table: "Coupons",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxDiscountAmount",
                schema: "PaymentsModule",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "MinPaymentAmount",
                schema: "PaymentsModule",
                table: "Coupons");
        }
    }
}
