using FluentAssertions;
using GloboTicket.Application.DTOs;
using GloboTicket.Domain.Entities;
using GloboTicket.Infrastructure.Data;
using GloboTicket.Infrastructure.Services;
using GloboTicket.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GloboTicket.IntegrationTests.Services;

/// <summary>
/// Integration tests for ShowService verifying show retrieval operations against a real SQL Server database.
/// Uses Testcontainers to spin up SQL Server instances for true integration testing.
/// Tests focus on the view show feature including tenant isolation through Venue relationship.
/// </summary>
public class ShowServiceIntegrationTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    private readonly int _testTenantId;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowServiceIntegrationTests"/> class.
    /// </summary>
    /// <param name="fixture">The database fixture providing the SQL Server container.</param>
    public ShowServiceIntegrationTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _testTenantId = _fixture.GenerateRandomTenantId();
    }

    [Fact]
    public async Task GetShowByGuid_WithValidGuid_ReturnsShowWithActAndVenue()
    {
        // Arrange
        var (showGuid, _, _) = await CreateTestShowAsync();

        // Act
        using var context = CreateDbContext(_fixture.ConnectionString, _testTenantId);
        var service = new ShowService(context);
        var result = await service.GetByGuidAsync(showGuid);

        // Assert
        result.Should().NotBeNull();
        result!.ShowGuid.Should().Be(showGuid);
        result.ActName.Should().NotBeNullOrEmpty();
        result.VenueName.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetShowByGuid_WithValidGuid_ReturnsShowDto()
    {
        // Arrange
        var (showGuid, _, _) = await CreateTestShowAsync();

        // Act
        using var context = CreateDbContext(_fixture.ConnectionString, _testTenantId);
        var service = new ShowService(context);
        var result = await service.GetByGuidAsync(showGuid);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ShowDto>();
        result!.Id.Should().BeGreaterThan(0);
        result.ShowGuid.Should().Be(showGuid);
    }

    [Fact]
    public async Task GetShowByGuid_WithNonExistentGuid_ReturnsNull()
    {
        // Arrange
        var nonExistentGuid = Guid.NewGuid();

        // Act
        using var context = CreateDbContext(_fixture.ConnectionString, _testTenantId);
        var service = new ShowService(context);
        var result = await service.GetByGuidAsync(nonExistentGuid);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetShowByGuid_IncludesActName()
    {
        // Arrange
        var (showGuid, actName, _) = await CreateTestShowAsync();

        // Act
        using var context = CreateDbContext(_fixture.ConnectionString, _testTenantId);
        var service = new ShowService(context);
        var result = await service.GetByGuidAsync(showGuid);

        // Assert
        result.Should().NotBeNull();
        result!.ActName.Should().Be(actName);
    }

    [Fact]
    public async Task GetShowByGuid_IncludesVenueName()
    {
        // Arrange
        var (showGuid, _, venueName) = await CreateTestShowAsync();

        // Act
        using var context = CreateDbContext(_fixture.ConnectionString, _testTenantId);
        var service = new ShowService(context);
        var result = await service.GetByGuidAsync(showGuid);

        // Assert
        result.Should().NotBeNull();
        result!.VenueName.Should().Be(venueName);
    }

    [Fact]
    public async Task GetShowByGuid_IncludesVenueCapacity()
    {
        // Arrange
        var (showGuid, _, _) = await CreateTestShowAsync();

        // Act
        using var context = CreateDbContext(_fixture.ConnectionString, _testTenantId);
        var service = new ShowService(context);
        var result = await service.GetByGuidAsync(showGuid);

        // Assert
        result.Should().NotBeNull();
        result!.VenueCapacity.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetShowByGuid_IncludesStartTime()
    {
        // Arrange
        var (showGuid, _, _) = await CreateTestShowAsync();

        // Act
        using var context = CreateDbContext(_fixture.ConnectionString, _testTenantId);
        var service = new ShowService(context);
        var result = await service.GetByGuidAsync(showGuid);

        // Assert
        result.Should().NotBeNull();
        result!.StartTime.Should().BeAfter(DateTimeOffset.UtcNow);
    }

    [Fact]
    public async Task GetShowByGuid_IncludesTicketCount()
    {
        // Arrange
        var (showGuid, _, _) = await CreateTestShowAsync();

        // Act
        using var context = CreateDbContext(_fixture.ConnectionString, _testTenantId);
        var service = new ShowService(context);
        var result = await service.GetByGuidAsync(showGuid);

        // Assert
        result.Should().NotBeNull();
        result!.TicketCount.Should().BeGreaterThan(0);
    }

    /// <summary>
    /// Creates a test show with associated tenant, venue, and act.
    /// </summary>
    /// <returns>A tuple containing the show GUID, act name, and venue name.</returns>
    private async Task<(Guid showGuid, string actName, string venueName)> CreateTestShowAsync()
    {
        using var setupContext = CreateDbContext(_fixture.ConnectionString, _testTenantId);

        // Create tenant
        var uniqueId = Guid.NewGuid().ToString()[..8];
        var tenant = new Tenant
        {
            TenantIdentifier = $"test-tenant-{uniqueId}",
            Name = $"Test Tenant {uniqueId}",
            Slug = $"test-tenant-{uniqueId}"
        };
        setupContext.Tenants.Add(tenant);
        await setupContext.SaveChangesAsync();

        // Create venue
        var venueName = $"Test Venue {uniqueId}";
        var venue = new Venue
        {
            TenantId = tenant.Id,
            VenueGuid = Guid.NewGuid(),
            Name = venueName,
            Address = "123 Test St",
            SeatingCapacity = 1000,
            Description = "Test venue description"
        };
        setupContext.Venues.Add(venue);
        await setupContext.SaveChangesAsync();

        // Create act
        var actName = $"Test Act {uniqueId}";
        var act = new Act
        {
            TenantId = tenant.Id,
            ActGuid = Guid.NewGuid(),
            Name = actName
        };
        setupContext.Acts.Add(act);
        await setupContext.SaveChangesAsync();

        // Create show
        var showGuid = Guid.NewGuid();
        var show = new Show
        {
            ShowGuid = showGuid,
            VenueId = venue.Id,
            Venue = venue,
            ActId = act.Id,
            Act = act,
            TicketCount = 500,
            StartTime = DateTimeOffset.UtcNow.AddDays(30)
        };
        setupContext.Shows.Add(show);
        await setupContext.SaveChangesAsync();

        return (showGuid, actName, venueName);
    }

    /// <summary>
    /// Creates and configures a GloboTicketDbContext for integration testing.
    /// </summary>
    /// <param name="connectionString">The database connection string.</param>
    /// <param name="tenantId">The tenant ID for the test context.</param>
    /// <returns>A configured GloboTicketDbContext instance.</returns>
    private static GloboTicketDbContext CreateDbContext(string connectionString, int? tenantId)
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
