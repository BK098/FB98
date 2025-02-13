using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FB98.Modules.Catalog.DataAccess.Data.Migrations
{
	/// <inheritdoc />
	public partial class Init_v2 : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<string>(
				name: "Image",
				schema: "CatalogModule",
				table: "Products",
				type: "text",
				nullable: true);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "Image",
				schema: "CatalogModule",
				table: "Products");
		}
	}
}
