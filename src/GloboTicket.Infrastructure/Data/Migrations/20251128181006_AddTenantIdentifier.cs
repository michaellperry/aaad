using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GloboTicket.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantIdentifier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add column as nullable first
            migrationBuilder.AddColumn<string>(
                name: "TenantIdentifier",
                table: "Tenants",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            // Populate existing rows with unique identifiers based on their Name
            migrationBuilder.Sql(@"
                UPDATE Tenants
                SET TenantIdentifier = LOWER(REPLACE(Name, ' ', '-')) + '-' + CAST(Id AS VARCHAR(10))
                WHERE TenantIdentifier IS NULL
            ");

            // Make column non-nullable
            migrationBuilder.AlterColumn<string>(
                name: "TenantIdentifier",
                table: "Tenants",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false);

            // Create unique index
            migrationBuilder.CreateIndex(
                name: "IX_Tenants_TenantIdentifier",
                table: "Tenants",
                column: "TenantIdentifier",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tenants_TenantIdentifier",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "TenantIdentifier",
                table: "Tenants");
        }
    }
}
