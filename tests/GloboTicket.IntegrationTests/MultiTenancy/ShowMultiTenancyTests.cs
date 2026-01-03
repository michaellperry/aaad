using FluentAssertions;
using GloboTicket.Domain.Entities;
using GloboTicket.Infrastructure.Data;
using GloboTicket.Infrastructure.Services;
using GloboTicket.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GloboTicket.IntegrationTests.MultiTenancy;

/// <summary>
/// Integration tests verifying multi-tenancy isolation for shows.
/// Shows inherit tenant context through their Venue relationship.
/// Tests ensure shows are properly filtered by tenant and cross-tenant access is prevented.
/// </summary>
public class ShowMultiTenancyTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    private readonly int _tenantAId;
    private readonly int _tenantBId;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowMultiTenancyTests"/> class.
    /// </summary>
    /// <param name="fixture">The database fixture providing the SQL Server container.</param>
    public ShowMultiTenancyTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _tenantAId = _fixture.GenerateRandomTenantId();
        _tenantBId = _fixture.GenerateRandomTenantId();
    }

    [Fact]
    public async Task GetShowByGuid_InTenantA_NotAccessibleToTenantB()
    {
        // Arrange - Create show in Tenant A
        var showGuid = await CreateShowInTenantAsync(_tenantAId);

        // Act - Try to access from Tenant B context
        using var tenantBContext = CreateDbContext(_fixture.ConnectionString, _tenantBId);
        var service = new ShowService(tenantBContext);
        var result = await service.GetByGuidAsync(showGuid);

        // Assert - Show should not be accessible from Tenant B
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetShowByGuid_ForShowInOtherTenantVenue_ReturnsNull()
    {
        // Arrange - Create show in Tenant A's venue
        var showGuid = await CreateShowInTenantAsync(_tenantAId);

        // Verify show exists in Tenant A context
        using (var tenantAContext = CreateDbContext(_fixture.ConnectionString, _tenantAId))
        {
            var serviceA = new ShowService(tenantAContext);
            var showInTenantA = await serviceA.GetByGuidAsync(showGuid);
            showInTenantA.Should().NotBeNull("show should exist in Tenant A");
        }

        // Act - Try to access from Tenant B context
        using var tenantBContext = CreateDbContext(_fixture.ConnectionString, _tenantBId);
        var serviceB = new ShowService(tenantBContext);
        var result = await serviceB.GetByGuidAsync(showGuid);

        // Assert - Show should not be accessible from Tenant B
        result.Should().BeNull("show belongs to Tenant A's venue and should not be visible to Tenant B");
    }

    [Fact]
    public async Task GetShowsByActGuid_ReturnsOnlyShowsInCurrentTenantVenues()
    {
        // Arrange - Create act in Tenant A
        var (actGuid, venueAGuid, venueBGuid) = await CreateActWithShowsInMultipleTenantsAsync();

        // Act - Get shows for act from Tenant A context
        using var tenantAContext = CreateDbContext(_fixture.ConnectionString, _tenantAId);
        var serviceA = new ShowService(tenantAContext);
        var showsInTenantA = await serviceA.GetShowsByActGuidAsync(actGuid);

        // Assert - Should only see shows in Tenant A's venues
        var showsList = showsInTenantA.ToList();
        showsList.Should().NotBeEmpty();
        showsList.Should().OnlyContain(s => s.VenueGuid == venueAGuid, 
            "only shows in Tenant A's venues should be visible");
    }

    /// <summary>
    /// Creates a show in the specified tenant's venue.
    /// </summary>
    /// <param name="tenantId">The tenant ID to create the show for.</param>
    /// <returns>The GUID of the created show.</returns>
    private async Task<Guid> CreateShowInTenantAsync(int tenantId)
    {
        using var setupContext = CreateDbContext(_fixture.ConnectionString, tenantId);

        // Create tenant
        var uniqueId = Guid.NewGuid().ToString()[..8];
        var tenant = new Tenant
        {
            TenantIdentifier = $"tenant-{tenantId}-{uniqueId}",
            Name = $"Tenant {tenantId}",
            Slug = $"tenant-{tenantId}-{uniqueId}"
        };
        setupContext.Tenants.Add(tenant);
        await setupContext.SaveChangesAsync();

        // Create venue in this tenant
        var venue = new Venue
        {
            TenantId = tenant.Id,
            VenueGuid = Guid.NewGuid(),
            Name = $"Venue {tenantId} {uniqueId}",
            Address = "123 Test St",
            SeatingCapacity = 1000,
            Description = "Test venue"
        };
        setupContext.Venues.Add(venue);
        await setupContext.SaveChangesAsync();

        // Create act in this tenant
        var act = new Act
        {
            TenantId = tenant.Id,
            ActGuid = Guid.NewGuid(),
            Name = $"Act {tenantId} {uniqueId}"
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

        return showGuid;
    }

    /// <summary>
    /// Creates an act with shows in venues from multiple tenants.
    /// This simulates a scenario where an act might perform at venues in different tenants.
    /// </summary>
    /// <returns>A tuple containing the act GUID and venue GUIDs for both tenants.</returns>
    private async Task<(Guid actGuid, Guid venueAGuid, Guid venueBGuid)> CreateActWithShowsInMultipleTenantsAsync()
    {
        var uniqueId = Guid.NewGuid().ToString()[..8];
        var actGuid = Guid.NewGuid();

        // Create Tenant A with venue and show
        Guid venueAGuid;
        using (var contextA = CreateDbContext(_fixture.ConnectionString, _tenantAId))
        {
            var tenantA = new Tenant
            {
                TenantIdentifier = $"tenant-a-{uniqueId}",
                Name = "Tenant A",
                Slug = $"tenant-a-{uniqueId}"
            };
            contextA.Tenants.Add(tenantA);
            await contextA.SaveChangesAsync();

            var venueA = new Venue
            {
                TenantId = tenantA.Id,
                VenueGuid = Guid.NewGuid(),
                Name = $"Venue A {uniqueId}",
                Address = "123 A St",
                SeatingCapacity = 1000,
                Description = "Venue A"
            };
            contextA.Venues.Add(venueA);
            await contextA.SaveChangesAsync();
            venueAGuid = venueA.VenueGuid;

            var actA = new Act
            {
                TenantId = tenantA.Id,
                ActGuid = actGuid,
                Name = $"Shared Act {uniqueId}"
            };
            contextA.Acts.Add(actA);
            await contextA.SaveChangesAsync();

            var showA = new Show
            {
                ShowGuid = Guid.NewGuid(),
                VenueId = venueA.Id,
                Venue = venueA,
                ActId = actA.Id,
                Act = actA,
                TicketCount = 500,
                StartTime = DateTimeOffset.UtcNow.AddDays(30)
            };
            contextA.Shows.Add(showA);
            await contextA.SaveChangesAsync();
        }

        // Create Tenant B with venue and show (same act GUID but different tenant)
        Guid venueBGuid;
        using (var contextB = CreateDbContext(_fixture.ConnectionString, _tenantBId))
        {
            var tenantB = new Tenant
            {
                TenantIdentifier = $"tenant-b-{uniqueId}",
                Name = "Tenant B",
                Slug = $"tenant-b-{uniqueId}"
            };
            contextB.Tenants.Add(tenantB);
            await contextB.SaveChangesAsync();

            var venueB = new Venue
            {
                TenantId = tenantB.Id,
                VenueGuid = Guid.NewGuid(),
                Name = $"Venue B {uniqueId}",
                Address = "456 B St",
                SeatingCapacity = 2000,
                Description = "Venue B"
            };
            contextB.Venues.Add(venueB);
            await contextB.SaveChangesAsync();
            venueBGuid = venueB.VenueGuid;

            var actB = new Act
            {
                TenantId = tenantB.Id,
                ActGuid = actGuid, // Same GUID as actA but in different tenant
                Name = $"Shared Act {uniqueId}"
            };
            contextB.Acts.Add(actB);
            await contextB.SaveChangesAsync();

            var showB = new Show
            {
                ShowGuid = Guid.NewGuid(),
                VenueId = venueB.Id,
                Venue = venueB,
                ActId = actB.Id,
                Act = actB,
                TicketCount = 800,
                StartTime = DateTimeOffset.UtcNow.AddDays(45)
            };
            contextB.Shows.Add(showB);
            await contextB.SaveChangesAsync();
        }

        return (actGuid, venueAGuid, venueBGuid);
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
