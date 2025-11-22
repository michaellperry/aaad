using FluentAssertions;
using GloboTicket.Application.DTOs;
using GloboTicket.Infrastructure.Data;
using GloboTicket.Infrastructure.Services;
using GloboTicket.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

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
        
        var createDto = new CreateTenantDto
        {
            Name = "Test Tenant",
            Slug = "test-tenant"
        };

        // Act
        var result = await service.CreateTenantAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Name.Should().Be("Test Tenant");
        result.Slug.Should().Be("test-tenant");
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
        
        var createDto = new CreateTenantDto
        {
            Name = "Get By ID Test",
            Slug = "get-by-id-test"
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
        result.Slug.Should().Be("get-by-id-test");
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
        
        var createDto = new CreateTenantDto
        {
            Name = "Get By Slug Test",
            Slug = "get-by-slug-test"
        };
        var created = await setupService.CreateTenantAsync(createDto);

        // Act
        using var context = CreateDbContext(_fixture.ConnectionString, _testTenantId);
        var service = new TenantService(context);
        var result = await service.GetTenantBySlugAsync("get-by-slug-test");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(created.Id);
        result.Name.Should().Be("Get By Slug Test");
        result.Slug.Should().Be("get-by-slug-test");
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
        
        var tenant1 = new CreateTenantDto
        {
            Name = "Tenant One",
            Slug = "tenant-one"
        };
        var tenant2 = new CreateTenantDto
        {
            Name = "Tenant Two",
            Slug = "tenant-two"
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
            .UseSqlServer(connectionString)
            .Options;

        var tenantContext = new TestTenantContext(tenantId);
        var context = new GloboTicketDbContext(options, tenantContext);
        
        // Ensure database is created for this test
        context.Database.EnsureCreated();
        
        return context;
    }
}