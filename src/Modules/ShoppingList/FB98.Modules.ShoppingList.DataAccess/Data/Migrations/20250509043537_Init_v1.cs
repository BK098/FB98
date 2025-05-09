using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FB98.Modules.ShoppingList.DataAccess.Data.Migrations
{
    /// <inheritdoc />
    public partial class Init_v1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ShoppingListModule");

            migrationBuilder.CreateTable(
                name: "Todos",
                schema: "ShoppingListModule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Todos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TodoItems",
                schema: "ShoppingListModule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true),
                    TodoId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Unit = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    PriceAtTime = table.Column<decimal>(type: "numeric", nullable: false),
                    IsChecked = table.Column<bool>(type: "boolean", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TodoItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TodoItems_Todos_TodoId",
                        column: x => x.TodoId,
                        principalSchema: "ShoppingListModule",
                        principalTable: "Todos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_TodoId",
                schema: "ShoppingListModule",
                table: "TodoItems",
                column: "TodoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TodoItems",
                schema: "ShoppingListModule");

            migrationBuilder.DropTable(
                name: "Todos",
                schema: "ShoppingListModule");
        }
    }
}
