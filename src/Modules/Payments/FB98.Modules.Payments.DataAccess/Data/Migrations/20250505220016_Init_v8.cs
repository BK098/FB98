using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FB98.Modules.Payments.DataAccess.Data.Migrations
{
    /// <inheritdoc />
    public partial class Init_v8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookingId",
                schema: "PaymentsModule",
                table: "PaymentTransactions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BookingId",
                schema: "PaymentsModule",
                table: "PaymentTransactions",
                type: "uuid",
                nullable: true);
        }
    }
}
