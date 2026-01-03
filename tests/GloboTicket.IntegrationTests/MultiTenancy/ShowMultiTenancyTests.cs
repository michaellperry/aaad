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

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowMultiTenancyTests"/> class.
    /// </summary>
    /// <param name="fixture">The database fixture providing the SQL Server container.</param>
    public ShowMultiTenancyTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetShowByGuid_InTenantA_NotAccessibleToTenantB()
    {
        // Arrange - Create show in Tenant A
        var (showGuid, tenantAId, tenantBId) = await CreateShowInTwoTenantsAsync();

        // Act - Try to access from Tenant B context
        var (tenantBContext, tenantBTenantContext) = _fixture.CreateDbContextWithTenant(_fixture.ConnectionString, tenantBId);
        using (tenantBContext)
        {
            var service = new ShowService(tenantBContext, tenantBTenantContext);
            var result = await service.GetByGuidAsync(showGuid);

            // Assert - Show should not be accessible from Tenant B
            result.Should().BeNull();
        }
    }

    [Fact]
    public async Task GetShowByGuid_ForShowInOtherTenantVenue_ReturnsNull()
    {
        // Arrange - Create show in Tenant A's venue
        var (showGuid, tenantAId, tenantBId) = await CreateShowInTwoTenantsAsync();

        // Verify show exists in Tenant A context
        var (tenantAContext, tenantATenantContext) = _fixture.CreateDbContextWithTenant(_fixture.ConnectionString, tenantAId);
        using (tenantAContext)
        {
            var serviceA = new ShowService(tenantAContext, tenantATenantContext);
            var showInTenantA = await serviceA.GetByGuidAsync(showGuid);
            showInTenantA.Should().NotBeNull("show should exist in Tenant A");
        }

        // Act - Try to access from Tenant B context
        var (tenantBContext, tenantBTenantContext) = _fixture.CreateDbContextWithTenant(_fixture.ConnectionString, tenantBId);
        using (tenantBContext)
        {
            var serviceB = new ShowService(tenantBContext, tenantBTenantContext);
            var result = await serviceB.GetByGuidAsync(showGuid);

            // Assert - Show should not be accessible from Tenant B
            result.Should().BeNull("show belongs to Tenant A's venue and should not be visible to Tenant B");
        }
    }

    [Fact]
    public async Task GetShowsByActGuid_ReturnsOnlyShowsInCurrentTenantVenues()
    {
        // Arrange - Create act in Tenant A
        var (actGuid, venueAGuid, venueBGuid, tenantAId, tenantBId) = await CreateActWithShowsInMultipleTenantsAsync();

        // Act - Get shows for act from Tenant A context
        var (tenantAContext, tenantATenantContext) = _fixture.CreateDbContextWithTenant(_fixture.ConnectionString, tenantAId);
        using (tenantAContext)
        {
            var serviceA = new ShowService(tenantAContext, tenantATenantContext);
            var showsInTenantA = await serviceA.GetShowsByActGuidAsync(actGuid);

            // Assert - Should only see shows in Tenant A's venues
            var showsList = showsInTenantA.ToList();
            showsList.Should().NotBeEmpty();
            showsList.Should().OnlyContain(s => s.VenueGuid == venueAGuid,
                "only shows in Tenant A's venues should be visible");
        }

    }

    /// <summary>
    /// Creates a show in two different tenant contexts for testing tenant isolation.
    /// </summary>
    /// <returns>A tuple containing the show GUID and the actual database tenant IDs for tenants A and B.</returns>
    private async Task<(Guid showGuid, int tenantAId, int tenantBId)> CreateShowInTwoTenantsAsync()
    {
        // Use null context for setup to avoid global query filter issues
        using var setupContext = _fixture.CreateDbContext(_fixture.ConnectionString, null);

        // Create Tenant A
        var uniqueIdA = Guid.NewGuid().ToString()[..8];
        var tenantA = new Tenant
        {
            TenantIdentifier = $"test-tenant-a-{uniqueIdA}",
            Name = $"Test Tenant A {uniqueIdA}",
            Slug = $"test-tenant-a-{uniqueIdA}",
            IsActive = true
        };
        setupContext.Tenants.Add(tenantA);
        await setupContext.SaveChangesAsync();

        // Create venue in Tenant A
        var venueGuid = Guid.NewGuid();
        var venue = new Venue
        {
            TenantId = tenantA.Id,
            VenueGuid = venueGuid,
            Name = "Venue A",
            Address = "123 Test St",
            SeatingCapacity = 1000,
            Description = "Test venue"
        };
        setupContext.Venues.Add(venue);
        await setupContext.SaveChangesAsync();

        // Create act in Tenant A
        var actGuid = Guid.NewGuid();
        var act = new Act
        {
            TenantId = tenantA.Id,
            ActGuid = actGuid,
            Name = "Act A"
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

        // Create Tenant B for isolation testing
        var uniqueIdB = Guid.NewGuid().ToString()[..8];
        var tenantB = new Tenant
        {
            TenantIdentifier = $"test-tenant-b-{uniqueIdB}",
            Name = $"Test Tenant B {uniqueIdB}",
            Slug = $"test-tenant-b-{uniqueIdB}",
            IsActive = true
        };
        setupContext.Tenants.Add(tenantB);
        await setupContext.SaveChangesAsync();

        return (showGuid, tenantA.Id, tenantB.Id);
    }

    /// <summary>
    /// Creates an act with shows in venues from multiple tenants.
    /// This simulates a scenario where an act might perform at venues in different tenants.
    /// </summary>
    /// <returns>A tuple containing the act GUID, venue GUIDs for both tenants, and the actual database tenant IDs.</returns>
    private async Task<(Guid actGuid, Guid venueAGuid, Guid venueBGuid, int tenantAId, int tenantBId)> CreateActWithShowsInMultipleTenantsAsync()
    {
        var uniqueId = Guid.NewGuid().ToString()[..8];
        var actGuid = Guid.NewGuid();

        // Create Tenant A with venue and show
        Guid venueAGuid;
        int tenantAId;
        using (var contextA = _fixture.CreateDbContext(_fixture.ConnectionString, null))
        {
            // Create tenant A directly in this context
            var tenantA = new Tenant
            {
                TenantIdentifier = $"test-tenant-a-{uniqueId}",
                Name = $"Test Tenant A {uniqueId}",
                Slug = $"test-tenant-a-{uniqueId}",
                IsActive = true
            };
            contextA.Tenants.Add(tenantA);
            await contextA.SaveChangesAsync();
            tenantAId = tenantA.Id;

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
        int tenantBId;
        using (var contextB = _fixture.CreateDbContext(_fixture.ConnectionString, null))
        {
            // Create tenant B directly in this context
            var tenantB = new Tenant
            {
                TenantIdentifier = $"test-tenant-b-{uniqueId}",
                Name = $"Test Tenant B {uniqueId}",
                Slug = $"test-tenant-b-{uniqueId}",
                IsActive = true
            };
            contextB.Tenants.Add(tenantB);
            await contextB.SaveChangesAsync();
            tenantBId = tenantB.Id;

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

        return (actGuid, venueAGuid, venueBGuid, tenantAId, tenantBId);
    }
}
