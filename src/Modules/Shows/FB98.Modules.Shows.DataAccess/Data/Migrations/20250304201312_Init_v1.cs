using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FB98.Modules.Shows.DataAccess.Data.Migrations
{
    /// <inheritdoc />
    public partial class Init_v1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ShowModule");

            migrationBuilder.CreateTable(
                name: "FeatureTypes",
                schema: "ShowModule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeatureTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShowStatuses",
                schema: "ShowModule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShowStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Features",
                schema: "ShowModule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    FeatureTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Features", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Features_FeatureTypes_FeatureTypeId",
                        column: x => x.FeatureTypeId,
                        principalSchema: "ShowModule",
                        principalTable: "FeatureTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Shows",
                schema: "ShowModule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ShowStatusId = table.Column<Guid>(type: "uuid", nullable: false),
                    MovieId = table.Column<Guid>(type: "uuid", nullable: false),
                    MovieTitle = table.Column<string>(type: "text", nullable: false),
                    MovieRuntimeMinutes = table.Column<int>(type: "integer", nullable: false),
                    CinemaHallId = table.Column<Guid>(type: "uuid", nullable: false),
                    CinemaHallName = table.Column<string>(type: "text", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shows_ShowStatuses_ShowStatusId",
                        column: x => x.ShowStatusId,
                        principalSchema: "ShowModule",
                        principalTable: "ShowStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShowFeatures",
                schema: "ShowModule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ShowId = table.Column<Guid>(type: "uuid", nullable: false),
                    FeatureId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShowFeatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShowFeatures_Features_FeatureId",
                        column: x => x.FeatureId,
                        principalSchema: "ShowModule",
                        principalTable: "Features",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShowFeatures_Shows_ShowId",
                        column: x => x.ShowId,
                        principalSchema: "ShowModule",
                        principalTable: "Shows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Features_FeatureTypeId",
                schema: "ShowModule",
                table: "Features",
                column: "FeatureTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ShowFeatures_FeatureId",
                schema: "ShowModule",
                table: "ShowFeatures",
                column: "FeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_ShowFeatures_ShowId",
                schema: "ShowModule",
                table: "ShowFeatures",
                column: "ShowId");

            migrationBuilder.CreateIndex(
                name: "IX_Shows_ShowStatusId",
                schema: "ShowModule",
                table: "Shows",
                column: "ShowStatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShowFeatures",
                schema: "ShowModule");

            migrationBuilder.DropTable(
                name: "Features",
                schema: "ShowModule");

            migrationBuilder.DropTable(
                name: "Shows",
                schema: "ShowModule");

            migrationBuilder.DropTable(
                name: "FeatureTypes",
                schema: "ShowModule");

            migrationBuilder.DropTable(
                name: "ShowStatuses",
                schema: "ShowModule");
        }
    }
}
