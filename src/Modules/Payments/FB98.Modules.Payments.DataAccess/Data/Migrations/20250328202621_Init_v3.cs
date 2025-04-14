using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FB98.Modules.Payments.DataAccess.Data.Migrations
{
    /// <inheritdoc />
    public partial class Init_v3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Coupons",
                schema: "PaymentsModule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<decimal>(type: "numeric", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MaxUsage = table.Column<int>(type: "integer", nullable: false),
                    UsageCount = table.Column<int>(type: "integer", nullable: false),
                    IsLimited = table.Column<bool>(type: "boolean", nullable: false),
                    IsDiscountPercentage = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CouponApplications",
                schema: "PaymentsModule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CouponId = table.Column<Guid>(type: "uuid", nullable: false),
                    PaymentTransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CouponApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CouponApplications_Coupons_CouponId",
                        column: x => x.CouponId,
                        principalSchema: "PaymentsModule",
                        principalTable: "Coupons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CouponApplications_PaymentTransactions_PaymentTransactionId",
                        column: x => x.PaymentTransactionId,
                        principalSchema: "PaymentsModule",
                        principalTable: "PaymentTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CouponApplications_CouponId",
                schema: "PaymentsModule",
                table: "CouponApplications",
                column: "CouponId");

            migrationBuilder.CreateIndex(
                name: "IX_CouponApplications_PaymentTransactionId",
                schema: "PaymentsModule",
                table: "CouponApplications",
                column: "PaymentTransactionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CouponApplications",
                schema: "PaymentsModule");

            migrationBuilder.DropTable(
                name: "Coupons",
                schema: "PaymentsModule");
        }
    }
}
