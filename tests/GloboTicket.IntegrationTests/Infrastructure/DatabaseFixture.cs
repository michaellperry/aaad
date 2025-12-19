using Testcontainers.MsSql;

namespace GloboTicket.IntegrationTests.Infrastructure;

/// <summary>
/// Fixture for managing SQL Server container lifecycle across integration tests.
/// Implements IAsyncLifetime for proper initialization and cleanup.
/// </summary>
public class DatabaseFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _container;

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
}
