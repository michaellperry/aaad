using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GloboTicket.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketSaleEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_Shows_TenantId_Id",
                table: "Shows",
                columns: new[] { "TenantId", "Id" });

            migrationBuilder.CreateTable(
                name: "TicketSales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketSaleGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShowId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketSales", x => x.Id);
                    table.UniqueConstraint("AK_TicketSales_TenantId_TicketSaleGuid", x => new { x.TenantId, x.TicketSaleGuid });
                    table.ForeignKey(
                        name: "FK_TicketSales_Shows_TenantId_ShowId",
                        columns: x => new { x.TenantId, x.ShowId },
                        principalTable: "Shows",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TicketSales_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TicketSales_TenantId_ShowId",
                table: "TicketSales",
                columns: new[] { "TenantId", "ShowId" });

            migrationBuilder.CreateIndex(
                name: "IX_TicketSales_TicketSaleGuid",
                table: "TicketSales",
                column: "TicketSaleGuid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicketSales");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Shows_TenantId_Id",
                table: "Shows");
        }
    }
}
