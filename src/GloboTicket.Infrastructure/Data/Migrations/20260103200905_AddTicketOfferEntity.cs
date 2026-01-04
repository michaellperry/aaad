using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GloboTicket.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketOfferEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TicketOffers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketOfferGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShowId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TicketCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketOffers", x => x.Id);
                    table.CheckConstraint("CK_TicketOffers_Price", "[Price] > 0");
                    table.CheckConstraint("CK_TicketOffers_TicketCount", "[TicketCount] > 0");
                    table.ForeignKey(
                        name: "FK_TicketOffers_Shows_ShowId",
                        column: x => x.ShowId,
                        principalTable: "Shows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TicketOffers_ShowId",
                table: "TicketOffers",
                column: "ShowId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketOffers_ShowId_CreatedAt",
                table: "TicketOffers",
                columns: new[] { "ShowId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_TicketOffers_TicketOfferGuid",
                table: "TicketOffers",
                column: "TicketOfferGuid",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicketOffers");
        }
    }
}
