using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FB98.Modules.ShoppingList.DataAccess.Data.Migrations
{
    /// <inheritdoc />
    public partial class Init_v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ParentItemId",
                schema: "ShoppingListModule",
                table: "TodoItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                schema: "ShoppingListModule",
                table: "TodoItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_ParentItemId",
                schema: "ShoppingListModule",
                table: "TodoItems",
                column: "ParentItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_TodoItems_TodoItems_ParentItemId",
                schema: "ShoppingListModule",
                table: "TodoItems",
                column: "ParentItemId",
                principalSchema: "ShoppingListModule",
                principalTable: "TodoItems",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TodoItems_TodoItems_ParentItemId",
                schema: "ShoppingListModule",
                table: "TodoItems");

            migrationBuilder.DropIndex(
                name: "IX_TodoItems_ParentItemId",
                schema: "ShoppingListModule",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "ParentItemId",
                schema: "ShoppingListModule",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "Quantity",
                schema: "ShoppingListModule",
                table: "TodoItems");
        }
    }
}
