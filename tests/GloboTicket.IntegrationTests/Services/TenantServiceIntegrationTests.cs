using FluentAssertions;
using GloboTicket.Application.DTOs;
using GloboTicket.Infrastructure.Data;
using GloboTicket.Application.Services;
using GloboTicket.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GloboTicket.IntegrationTests.Services;

/// <summary>
/// Integration tests for TenantService verifying CRUD operations against a real SQL Server database.
/// Uses Testcontainers to spin up SQL Server instances for true integration testing.
/// </summary>
public class TenantServiceIntegrationTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    private readonly int _testTenantId;

    /// <summary>
    /// Initializes a new instance of the <see cref="TenantServiceIntegrationTests"/> class.
    /// </summary>
    /// <param name="fixture">The database fixture providing the SQL Server container.</param>
    public TenantServiceIntegrationTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _testTenantId = _fixture.GenerateRandomTenantId();
    }

    [Fact]
    public async Task CreateTenant_WithValidData_SavesToDatabase()
    {
        // Arrange
        using var context = CreateDbContext(_fixture.ConnectionString, _testTenantId);
        var service = new TenantService(context);

        var uniqueId = Guid.NewGuid().ToString()[..8];
        var createDto = new CreateTenantDto
        {
            TenantIdentifier = $"test-tenant-{uniqueId}",
            Name = "Test Tenant",
            Slug = $"test-tenant-{uniqueId}"
        };

        // Act
        var result = await service.CreateTenantAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Name.Should().Be("Test Tenant");
        result.Slug.Should().Be($"test-tenant-{uniqueId}");
        result.IsActive.Should().BeTrue();
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        // Verify it was saved to the database
        using var verifyContext = CreateDbContext(_fixture.ConnectionString, _testTenantId);
        var savedTenant = await verifyContext.Tenants.FindAsync(result.Id);
        savedTenant.Should().NotBeNull();
        savedTenant!.Name.Should().Be("Test Tenant");
    }

    [Fact]
    public async Task GetTenantById_ExistingTenant_ReturnsTenant()
    {
        // Arrange
        using var setupContext = CreateDbContext(_fixture.ConnectionString, _testTenantId);
        var setupService = new TenantService(setupContext);

        var uniqueId = Guid.NewGuid().ToString()[..8];
        var createDto = new CreateTenantDto
        {
            TenantIdentifier = $"get-by-id-test-{uniqueId}",
            Name = "Get By ID Test",
            Slug = $"get-by-id-test-{uniqueId}"
        };
        var created = await setupService.CreateTenantAsync(createDto);

        // Act
        using var context = CreateDbContext(_fixture.ConnectionString, _testTenantId);
        var service = new TenantService(context);
        var result = await service.GetTenantByIdAsync(created.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(created.Id);
        result.Name.Should().Be("Get By ID Test");
        result.Slug.Should().Be($"get-by-id-test-{uniqueId}");
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task GetTenantById_NonExistentTenant_ReturnsNull()
    {
        // Arrange
        using var context = CreateDbContext(_fixture.ConnectionString, _testTenantId);
        var service = new TenantService(context);

        // Act
        var result = await service.GetTenantByIdAsync(99999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetTenantBySlug_ExistingTenant_ReturnsTenant()
    {
        // Arrange
        using var setupContext = CreateDbContext(_fixture.ConnectionString, _testTenantId);
        var setupService = new TenantService(setupContext);

        var uniqueId = Guid.NewGuid().ToString()[..8];
        var slug = $"get-by-slug-test-{uniqueId}";
        var createDto = new CreateTenantDto
        {
            TenantIdentifier = $"get-by-slug-test-{uniqueId}",
            Name = "Get By Slug Test",
            Slug = slug
        };
        var created = await setupService.CreateTenantAsync(createDto);

        // Act
        using var context = CreateDbContext(_fixture.ConnectionString, _testTenantId);
        var service = new TenantService(context);
        var result = await service.GetTenantBySlugAsync(slug);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(created.Id);
        result.Name.Should().Be("Get By Slug Test");
        result.Slug.Should().Be(slug);
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task GetTenantBySlug_NonExistentSlug_ReturnsNull()
    {
        // Arrange
        using var context = CreateDbContext(_fixture.ConnectionString, _testTenantId);
        var service = new TenantService(context);

        // Act
        var result = await service.GetTenantBySlugAsync("non-existent-slug");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllTenants_ReturnsAllTenants()
    {
        // Arrange
        using var setupContext = CreateDbContext(_fixture.ConnectionString, _testTenantId);
        var setupService = new TenantService(setupContext);

        var uniqueId1 = Guid.NewGuid().ToString()[..8];
        var uniqueId2 = Guid.NewGuid().ToString()[..8];
        var tenant1 = new CreateTenantDto
        {
            TenantIdentifier = $"tenant-one-{uniqueId1}",
            Name = "Tenant One",
            Slug = $"tenant-one-{uniqueId1}"
        };
        var tenant2 = new CreateTenantDto
        {
            TenantIdentifier = $"tenant-two-{uniqueId2}",
            Name = "Tenant Two",
            Slug = $"tenant-two-{uniqueId2}"
        };

        await setupService.CreateTenantAsync(tenant1);
        await setupService.CreateTenantAsync(tenant2);

        // Act
        using var context = CreateDbContext(_fixture.ConnectionString, _testTenantId);
        var service = new TenantService(context);
        var result = await service.GetAllTenantsAsync();

        // Assert
        var tenants = result.ToList();
        tenants.Should().HaveCountGreaterThanOrEqualTo(2);
        tenants.Should().Contain(t => t.Name == "Tenant One");
        tenants.Should().Contain(t => t.Name == "Tenant Two");
    }

    /// <summary>
    /// Creates and configures a GloboTicketDbContext for integration testing.
    /// </summary>
    /// <param name="connectionString">The database connection string.</param>
    /// <param name="tenantId">The tenant ID for the test context.</param>
    /// <returns>A configured GloboTicketDbContext instance.</returns>
    private static GloboTicketDbContext CreateDbContext(string connectionString, int tenantId)
    {
        var options = new DbContextOptionsBuilder<GloboTicketDbContext>()
            .UseSqlServer(connectionString, sqlOptions => sqlOptions.UseNetTopologySuite())
            .Options;

        var tenantContext = new TestTenantContext(tenantId);
        var context = new GloboTicketDbContext(options, tenantContext);

        // Apply migrations to ensure database schema matches production behavior
        context.Database.Migrate();

        return context;
    }
}
