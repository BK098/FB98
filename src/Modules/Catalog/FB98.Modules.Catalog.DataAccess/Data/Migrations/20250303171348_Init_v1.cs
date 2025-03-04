using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FB98.Modules.Catalog.DataAccess.Data.Migrations
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
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Image = table.Column<string>(type: "text", nullable: true),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false)
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
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Image = table.Column<string>(type: "text", nullable: true),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false)
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
                        onDelete: ReferentialAction.Restrict);
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
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComboProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "CatalogModule",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductDiscountRules",
                schema: "CatalogModule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<decimal>(type: "numeric", nullable: false),
                    IsCombo = table.Column<bool>(type: "boolean", nullable: false),
                    IsDiscountPercentage = table.Column<bool>(type: "boolean", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: true),
                    ComboId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductDiscountRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductDiscountRules_Combos_ComboId",
                        column: x => x.ComboId,
                        principalSchema: "CatalogModule",
                        principalTable: "Combos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductDiscountRules_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "CatalogModule",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductDiscountApplications",
                schema: "CatalogModule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsCombo = table.Column<bool>(type: "boolean", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    AppliedAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    RuleId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductDiscountApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductDiscountApplications_ProductDiscountRules_RuleId",
                        column: x => x.RuleId,
                        principalSchema: "CatalogModule",
                        principalTable: "ProductDiscountRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                name: "IX_ProductDiscountApplications_RuleId",
                schema: "CatalogModule",
                table: "ProductDiscountApplications",
                column: "RuleId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductDiscountRules_ComboId",
                schema: "CatalogModule",
                table: "ProductDiscountRules",
                column: "ComboId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductDiscountRules_ProductId",
                schema: "CatalogModule",
                table: "ProductDiscountRules",
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
                name: "ProductDiscountApplications",
                schema: "CatalogModule");

            migrationBuilder.DropTable(
                name: "ProductDiscountRules",
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
