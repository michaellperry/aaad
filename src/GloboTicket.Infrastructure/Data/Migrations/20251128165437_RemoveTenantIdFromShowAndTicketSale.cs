using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GloboTicket.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTenantIdFromShowAndTicketSale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shows_Acts_TenantId_ActId",
                table: "Shows");

            migrationBuilder.DropForeignKey(
                name: "FK_Shows_Tenants_TenantId",
                table: "Shows");

            migrationBuilder.DropForeignKey(
                name: "FK_Shows_Venues_TenantId_VenueId",
                table: "Shows");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketSales_Shows_TenantId_ShowId",
                table: "TicketSales");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketSales_Tenants_TenantId",
                table: "TicketSales");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Venues_TenantId_Id",
                table: "Venues");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_TicketSales_TenantId_TicketSaleGuid",
                table: "TicketSales");

            migrationBuilder.DropIndex(
                name: "IX_TicketSales_TenantId_ShowId",
                table: "TicketSales");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Shows_TenantId_Id",
                table: "Shows");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Shows_TenantId_ShowGuid",
                table: "Shows");

            migrationBuilder.DropIndex(
                name: "IX_Shows_TenantId_ActId",
                table: "Shows");

            migrationBuilder.DropIndex(
                name: "IX_Shows_TenantId_VenueId",
                table: "Shows");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Acts_TenantId_Id",
                table: "Acts");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "TicketSales");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Shows");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Shows_ShowGuid",
                table: "Shows",
                column: "ShowGuid");

            migrationBuilder.CreateIndex(
                name: "IX_TicketSales_ShowId",
                table: "TicketSales",
                column: "ShowId");

            migrationBuilder.CreateIndex(
                name: "IX_Shows_ActId",
                table: "Shows",
                column: "ActId");

            migrationBuilder.CreateIndex(
                name: "IX_Shows_VenueId",
                table: "Shows",
                column: "VenueId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shows_Acts_ActId",
                table: "Shows",
                column: "ActId",
                principalTable: "Acts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Shows_Venues_VenueId",
                table: "Shows",
                column: "VenueId",
                principalTable: "Venues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketSales_Shows_ShowId",
                table: "TicketSales",
                column: "ShowId",
                principalTable: "Shows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shows_Acts_ActId",
                table: "Shows");

            migrationBuilder.DropForeignKey(
                name: "FK_Shows_Venues_VenueId",
                table: "Shows");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketSales_Shows_ShowId",
                table: "TicketSales");

            migrationBuilder.DropIndex(
                name: "IX_TicketSales_ShowId",
                table: "TicketSales");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Shows_ShowGuid",
                table: "Shows");

            migrationBuilder.DropIndex(
                name: "IX_Shows_ActId",
                table: "Shows");

            migrationBuilder.DropIndex(
                name: "IX_Shows_VenueId",
                table: "Shows");

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "TicketSales",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Shows",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Venues_TenantId_Id",
                table: "Venues",
                columns: new[] { "TenantId", "Id" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_TicketSales_TenantId_TicketSaleGuid",
                table: "TicketSales",
                columns: new[] { "TenantId", "TicketSaleGuid" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Shows_TenantId_Id",
                table: "Shows",
                columns: new[] { "TenantId", "Id" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Shows_TenantId_ShowGuid",
                table: "Shows",
                columns: new[] { "TenantId", "ShowGuid" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Acts_TenantId_Id",
                table: "Acts",
                columns: new[] { "TenantId", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_TicketSales_TenantId_ShowId",
                table: "TicketSales",
                columns: new[] { "TenantId", "ShowId" });

            migrationBuilder.CreateIndex(
                name: "IX_Shows_TenantId_ActId",
                table: "Shows",
                columns: new[] { "TenantId", "ActId" });

            migrationBuilder.CreateIndex(
                name: "IX_Shows_TenantId_VenueId",
                table: "Shows",
                columns: new[] { "TenantId", "VenueId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Shows_Acts_TenantId_ActId",
                table: "Shows",
                columns: new[] { "TenantId", "ActId" },
                principalTable: "Acts",
                principalColumns: new[] { "TenantId", "Id" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Shows_Tenants_TenantId",
                table: "Shows",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Shows_Venues_TenantId_VenueId",
                table: "Shows",
                columns: new[] { "TenantId", "VenueId" },
                principalTable: "Venues",
                principalColumns: new[] { "TenantId", "Id" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketSales_Shows_TenantId_ShowId",
                table: "TicketSales",
                columns: new[] { "TenantId", "ShowId" },
                principalTable: "Shows",
                principalColumns: new[] { "TenantId", "Id" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketSales_Tenants_TenantId",
                table: "TicketSales",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
