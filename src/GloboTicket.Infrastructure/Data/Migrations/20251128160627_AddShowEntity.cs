using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GloboTicket.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddShowEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_Venues_TenantId_Id",
                table: "Venues",
                columns: new[] { "TenantId", "Id" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Acts_TenantId_Id",
                table: "Acts",
                columns: new[] { "TenantId", "Id" });

            migrationBuilder.CreateTable(
                name: "Shows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShowGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VenueId = table.Column<int>(type: "int", nullable: false),
                    ActId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shows", x => x.Id);
                    table.UniqueConstraint("AK_Shows_TenantId_ShowGuid", x => new { x.TenantId, x.ShowGuid });
                    table.ForeignKey(
                        name: "FK_Shows_Acts_TenantId_ActId",
                        columns: x => new { x.TenantId, x.ActId },
                        principalTable: "Acts",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Shows_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Shows_Venues_TenantId_VenueId",
                        columns: x => new { x.TenantId, x.VenueId },
                        principalTable: "Venues",
                        principalColumns: new[] { "TenantId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Shows_ShowGuid",
                table: "Shows",
                column: "ShowGuid");

            migrationBuilder.CreateIndex(
                name: "IX_Shows_TenantId_ActId",
                table: "Shows",
                columns: new[] { "TenantId", "ActId" });

            migrationBuilder.CreateIndex(
                name: "IX_Shows_TenantId_VenueId",
                table: "Shows",
                columns: new[] { "TenantId", "VenueId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Shows");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Venues_TenantId_Id",
                table: "Venues");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Acts_TenantId_Id",
                table: "Acts");
        }
    }
}
