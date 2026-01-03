using GloboTicket.Infrastructure.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace GloboTicket.Infrastructure.Data;

/// <summary>
/// Factory for creating DbContext instances during design-time operations (migrations, etc.).
/// This factory uses the MigrationConnection connection string to ensure migrations
/// are executed with the correct elevated privileges.
/// </summary>
public class GloboTicketDbContextFactory : IDesignTimeDbContextFactory<GloboTicketDbContext>
{
    /// <summary>
    /// Creates a DbContext instance for design-time operations.
    /// </summary>
    /// <param name="args">Command-line arguments (not used).</param>
    /// <returns>A configured GloboTicketDbContext instance.</returns>
    public GloboTicketDbContext CreateDbContext(string[] args)
    {
        // Build configuration from appsettings files
        // Design-time tools use the startup project (API) as the base path
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../GloboTicket.API"))
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        // Prefer MigrationConnection, fall back to DefaultConnection
        var connectionString = configuration.GetConnectionString("MigrationConnection")
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "No connection string found. Ensure MigrationConnection or DefaultConnection is configured in appsettings.json.");

        var optionsBuilder = new DbContextOptionsBuilder<GloboTicketDbContext>();
        optionsBuilder.UseSqlServer(connectionString, sqlOptions => sqlOptions.UseNetTopologySuite());

        // Design-time operations don't need tenant context (returns null for CurrentTenantId)
        var tenantContext = new DesignTimeTenantContext();

        return new GloboTicketDbContext(optionsBuilder.Options, tenantContext);
    }

    /// <summary>
    /// Dummy tenant context for design-time operations.
    /// Design-time operations don't need tenant filtering, so CurrentTenantId returns null.
    /// </summary>
    private class DesignTimeTenantContext : ITenantContext
    {
        /// <inheritdoc />
        public int? CurrentTenantId => null; // No tenant filtering during design-time
    }
}

