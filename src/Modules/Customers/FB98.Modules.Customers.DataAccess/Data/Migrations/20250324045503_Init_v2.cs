using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FB98.Modules.Customers.DataAccess.Data.Migrations
{
    /// <inheritdoc />
    public partial class Init_v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmountForUpgrade",
                schema: "CustomerModule",
                table: "Memberships",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalAmountForUpgrade",
                schema: "CustomerModule",
                table: "Memberships");
        }
    }
}
