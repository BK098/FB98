using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FB98.Modules.Catalog.DataAccess.Data.Mirgrations
{
	/// <inheritdoc />
	public partial class Init_v1 : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.EnsureSchema(
				name: "CatalogModule");

			migrationBuilder.CreateTable(
				name: "Categories",
				schema: "CatalogModule",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uuid", nullable: false),
					Name = table.Column<string>(type: "text", nullable: false),
					CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Categories", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "Combos",
				schema: "CatalogModule",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uuid", nullable: false),
					Name = table.Column<string>(type: "text", nullable: false),
					Description = table.Column<string>(type: "text", nullable: true),
					Price = table.Column<decimal>(type: "numeric", nullable: false),
					IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
					CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Combos", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "Products",
				schema: "CatalogModule",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uuid", nullable: false),
					Name = table.Column<string>(type: "text", nullable: false),
					Description = table.Column<string>(type: "text", nullable: true),
					Price = table.Column<decimal>(type: "numeric", nullable: false),
					IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
					CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
					CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Products", x => x.Id);
					table.ForeignKey(
						name: "FK_Products_Categories_CategoryId",
						column: x => x.CategoryId,
						principalSchema: "CatalogModule",
						principalTable: "Categories",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateTable(
				name: "ComboProducts",
				schema: "CatalogModule",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uuid", nullable: false),
					Quantity = table.Column<int>(type: "integer", nullable: false),
					ProductId = table.Column<Guid>(type: "uuid", nullable: false),
					ComboId = table.Column<Guid>(type: "uuid", nullable: false),
					CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
					UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_ComboProducts", x => x.Id);
					table.ForeignKey(
						name: "FK_ComboProducts_Combos_ComboId",
						column: x => x.ComboId,
						principalSchema: "CatalogModule",
						principalTable: "Combos",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
					table.ForeignKey(
						name: "FK_ComboProducts_Products_ProductId",
						column: x => x.ProductId,
						principalSchema: "CatalogModule",
						principalTable: "Products",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_ComboProducts_ComboId",
				schema: "CatalogModule",
				table: "ComboProducts",
				column: "ComboId");

			migrationBuilder.CreateIndex(
				name: "IX_ComboProducts_ProductId",
				schema: "CatalogModule",
				table: "ComboProducts",
				column: "ProductId");

			migrationBuilder.CreateIndex(
				name: "IX_Products_CategoryId",
				schema: "CatalogModule",
				table: "Products",
				column: "CategoryId");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "ComboProducts",
				schema: "CatalogModule");

			migrationBuilder.DropTable(
				name: "Combos",
				schema: "CatalogModule");

			migrationBuilder.DropTable(
				name: "Products",
				schema: "CatalogModule");

			migrationBuilder.DropTable(
				name: "Categories",
				schema: "CatalogModule");
		}
	}
}
