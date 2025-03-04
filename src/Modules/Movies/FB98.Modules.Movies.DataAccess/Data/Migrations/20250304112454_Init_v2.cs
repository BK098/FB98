using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FB98.Modules.Movies.DataAccess.Data.Migrations
{
    /// <inheritdoc />
    public partial class Init_v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Country",
                schema: "MovieModule",
                table: "Movies",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                schema: "MovieModule",
                table: "Movies");
        }
    }
}
