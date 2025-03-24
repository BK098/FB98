using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FB98.Modules.Tickets.DataAccess.Data.Migrations
{
    /// <inheritdoc />
    public partial class Init_v1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "TicketModule");

            migrationBuilder.CreateTable(
                name: "BookingSeatLocks",
                schema: "TicketModule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ShowId = table.Column<Guid>(type: "uuid", nullable: false),
                    SeatId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    LockedUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsPaymentInProgress = table.Column<bool>(type: "boolean", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingSeatLocks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BookingSeatStatuses",
                schema: "TicketModule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingSeatStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BookingStatuses",
                schema: "TicketModule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SeatPriceRules",
                schema: "TicketModule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SeatTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    DaysOfWeek = table.Column<int>(type: "integer", nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MinAge = table.Column<int>(type: "integer", nullable: true),
                    MaxAge = table.Column<int>(type: "integer", nullable: true),
                    CustomerType = table.Column<int>(type: "integer", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    IsActived = table.Column<bool>(type: "boolean", nullable: false),
                    IsHoliday = table.Column<bool>(type: "boolean", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeatPriceRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                schema: "TicketModule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    UserPhone = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    ShowId = table.Column<Guid>(type: "uuid", nullable: false),
                    HallId = table.Column<Guid>(type: "uuid", nullable: false),
                    HallName = table.Column<string>(type: "text", nullable: false),
                    ShowStart = table.Column<string>(type: "text", nullable: false),
                    ShowEnd = table.Column<string>(type: "text", nullable: false),
                    MovieTitle = table.Column<string>(type: "text", nullable: false),
                    StatusId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_BookingStatuses_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "TicketModule",
                        principalTable: "BookingStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingSeats",
                schema: "TicketModule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    SeatStatusId = table.Column<Guid>(type: "uuid", nullable: false),
                    SeatId = table.Column<Guid>(type: "uuid", nullable: false),
                    SeatTypeName = table.Column<string>(type: "text", nullable: false),
                    SeatPosition = table.Column<string>(type: "text", nullable: false),
                    IsReserved = table.Column<bool>(type: "boolean", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingSeats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingSeats_BookingSeatStatuses_SeatStatusId",
                        column: x => x.SeatStatusId,
                        principalSchema: "TicketModule",
                        principalTable: "BookingSeatStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingSeats_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalSchema: "TicketModule",
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SeatPriceApplications",
                schema: "TicketModule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SeatPriceRuleId = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingSeatId = table.Column<Guid>(type: "uuid", nullable: false),
                    AppliedPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeatPriceApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeatPriceApplications_BookingSeats_BookingSeatId",
                        column: x => x.BookingSeatId,
                        principalSchema: "TicketModule",
                        principalTable: "BookingSeats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SeatPriceApplications_SeatPriceRules_SeatPriceRuleId",
                        column: x => x.SeatPriceRuleId,
                        principalSchema: "TicketModule",
                        principalTable: "SeatPriceRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_StatusId",
                schema: "TicketModule",
                table: "Bookings",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingSeats_BookingId",
                schema: "TicketModule",
                table: "BookingSeats",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingSeats_SeatStatusId",
                schema: "TicketModule",
                table: "BookingSeats",
                column: "SeatStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_SeatPriceApplications_BookingSeatId",
                schema: "TicketModule",
                table: "SeatPriceApplications",
                column: "BookingSeatId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SeatPriceApplications_SeatPriceRuleId",
                schema: "TicketModule",
                table: "SeatPriceApplications",
                column: "SeatPriceRuleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingSeatLocks",
                schema: "TicketModule");

            migrationBuilder.DropTable(
                name: "SeatPriceApplications",
                schema: "TicketModule");

            migrationBuilder.DropTable(
                name: "BookingSeats",
                schema: "TicketModule");

            migrationBuilder.DropTable(
                name: "SeatPriceRules",
                schema: "TicketModule");

            migrationBuilder.DropTable(
                name: "BookingSeatStatuses",
                schema: "TicketModule");

            migrationBuilder.DropTable(
                name: "Bookings",
                schema: "TicketModule");

            migrationBuilder.DropTable(
                name: "BookingStatuses",
                schema: "TicketModule");
        }
    }
}
