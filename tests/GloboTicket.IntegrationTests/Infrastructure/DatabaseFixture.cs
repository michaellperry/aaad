using Testcontainers.MsSql;
using GloboTicket.Domain.Entities;
using GloboTicket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GloboTicket.IntegrationTests.Infrastructure;

/// <summary>
/// Fixture for managing SQL Server container lifecycle across integration tests.
/// Implements IAsyncLifetime for proper initialization and cleanup.
/// </summary>
public class DatabaseFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _container;
    private static readonly object _migrationLock = new object();

    // NOTE:
    // `mcr.microsoft.com/mssql/server:2022-latest` is currently failing to start on our CI/WSL2 Docker environment
    // with: "/opt/mssql/bin/sqlservr: The file archive [/opt/mssql/lib/system.netfx.sfp] is invalid".
    // Pin to a known-good, reproducible image tag to keep integration tests stable.
    private const string SqlServerImage = "mcr.microsoft.com/mssql/server:2022-CU15-ubuntu-22.04";

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseFixture"/> class.
    /// </summary>
    public DatabaseFixture()
    {
        _container = new MsSqlBuilder()
            .WithImage(SqlServerImage)
            .Build();
    }

    /// <summary>
    /// Initializes the fixture by starting the SQL Server container.
    /// </summary>
    public async Task InitializeAsync()
    {
        await _container.StartAsync();
    }

    /// <summary>
    /// Cleans up the fixture by stopping and disposing the SQL Server container.
    /// </summary>
    public Task DisposeAsync()
    {
        return _container.DisposeAsync().AsTask();
    }

    /// <summary>
    /// Gets the connection string for the SQL Server container.
    /// </summary>
    public string ConnectionString => _container.GetConnectionString();

    /// <summary>
    /// Generates a random tenant ID for test isolation.
    /// Each test class should use unique tenant IDs to prevent cross-contamination.
    /// </summary>
    /// <returns>A random tenant ID between 1000 and 9999.</returns>
    public int GenerateRandomTenantId()
    {
        return Random.Shared.Next(1000, 9999);
    }

    /// <summary>
    /// Creates a test tenant with unique identifier for test isolation.
    /// </summary>
    /// <param name="connectionString">The database connection string.</param>
    /// <param name="tenantId">The tenant ID for the test context.</param>
    /// <returns>The created tenant entity.</returns>
    public async Task<Tenant> CreateTestTenantAsync(string connectionString, int tenantId)
    {
        using var context = CreateDbContext(connectionString, null);
        
        var uniqueId = Guid.NewGuid().ToString()[..8];
        var tenant = new Tenant
        {
            TenantIdentifier = $"test-tenant-{tenantId}-{uniqueId}",
            Name = $"Test Tenant {tenantId} {uniqueId}",
            Slug = $"test-tenant-{tenantId}-{uniqueId}",
            IsActive = true
        };
        
        context.Tenants.Add(tenant);
        await context.SaveChangesAsync();
        
        return tenant;
    }

    /// <summary>
    /// Cleans up all test data for the specified tenant.
    /// </summary>
    /// <param name="connectionString">The database connection string.</param>
    /// <param name="tenantId">The tenant ID to clean up.</param>
    public async Task CleanupTestDataAsync(string connectionString, int tenantId)
    {
        using var context = CreateDbContext(connectionString, tenantId);
        
        // Delete in reverse dependency order to avoid foreign key constraint issues
        context.Shows.RemoveRange(context.Shows);
        context.Acts.RemoveRange(context.Acts);
        context.Venues.RemoveRange(context.Venues);
        context.Tenants.RemoveRange(context.Tenants.Where(t => t.Id == tenantId));
        
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Creates and configures a GloboTicketDbContext for integration testing.
    /// Applies migrations in a thread-safe manner.
    /// </summary>
    /// <param name="connectionString">The database connection string.</param>
    /// <param name="tenantId">The tenant ID for the test context.</param>
    /// <returns>A tuple containing the configured GloboTicketDbContext and ITenantContext.</returns>
    public (GloboTicketDbContext context, TestTenantContext tenantContext) CreateDbContextWithTenant(string connectionString, int? tenantId)
    {
        var options = new DbContextOptionsBuilder<GloboTicketDbContext>()
            .UseSqlServer(connectionString, sqlOptions => sqlOptions.UseNetTopologySuite())
            .Options;

        var tenantContext = new TestTenantContext(tenantId);
        var context = new GloboTicketDbContext(options, tenantContext);
        
        // Apply migrations in a thread-safe manner to avoid race conditions
        lock (_migrationLock)
        {
            context.Database.EnsureCreated();
        }
        
        return (context, tenantContext);
    }

    /// <summary>
    /// Creates and configures a GloboTicketDbContext for integration testing.
    /// Applies migrations in a thread-safe manner.
    /// </summary>
    /// <param name="connectionString">The database connection string.</param>
    /// <param name="tenantId">The tenant ID for the test context.</param>
    /// <returns>A configured GloboTicketDbContext instance.</returns>
    public GloboTicketDbContext CreateDbContext(string connectionString, int? tenantId)
    {
        var (context, _) = CreateDbContextWithTenant(connectionString, tenantId);
        return context;
    }

    /// <summary>
    /// Static method to create and configure a GloboTicketDbContext for integration testing.
    /// This method is kept for backward compatibility with existing test code.
    /// </summary>
    /// <param name="connectionString">The database connection string.</param>
    /// <param name="tenantId">The tenant ID for the test context.</param>
    /// <returns>A configured GloboTicketDbContext instance.</returns>
    public static GloboTicketDbContext CreateDbContextStatic(string connectionString, int? tenantId)
    {
        var options = new DbContextOptionsBuilder<GloboTicketDbContext>()
            .UseSqlServer(connectionString, sqlOptions => sqlOptions.UseNetTopologySuite())
            .Options;

        var tenantContext = new TestTenantContext(tenantId);
        var context = new GloboTicketDbContext(options, tenantContext);
        
        // Apply migrations in a thread-safe manner
        lock (_migrationLock)
        {
            context.Database.EnsureCreated();
        }
        
        return context;
    }
}
