using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FB98.Modules.Catalog.DataAccess.Data.Mirgrations
{
	/// <inheritdoc />
	public partial class Init_v3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                schema: "CatalogModule",
                table: "Combos",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                schema: "CatalogModule",
                table: "Combos");
        }
    }
}
