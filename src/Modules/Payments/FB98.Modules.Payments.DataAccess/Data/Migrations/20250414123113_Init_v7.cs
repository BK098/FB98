using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FB98.Modules.Payments.DataAccess.Data.Migrations
{
    /// <inheritdoc />
    public partial class Init_v7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "PaymentsModule",
                table: "Coupons",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                schema: "PaymentsModule",
                table: "Coupons");
        }
    }
}
