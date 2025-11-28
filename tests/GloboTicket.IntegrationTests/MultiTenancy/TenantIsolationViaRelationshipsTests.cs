using FluentAssertions;
using GloboTicket.Application.DTOs;
using GloboTicket.Infrastructure.Data;
using GloboTicket.Infrastructure.Services;
using GloboTicket.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GloboTicket.IntegrationTests.MultiTenancy;

/// <summary>
/// Integration tests verifying multi-tenant data isolation via relationships.
/// Tests the behavior where Show and TicketSale entities inherit tenant context
/// through their relationships rather than having their own TenantId column.
/// </summary>
public class TenantIsolationViaRelationshipsTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    private readonly int _tenant1Id;
    private readonly int _tenant2Id;

    /// <summary>
    /// Initializes a new instance of the <see cref="TenantIsolationViaRelationshipsTests"/> class.
    /// </summary>
    /// <param name="fixture">The database fixture providing the SQL Server container.</param>
    public TenantIsolationViaRelationshipsTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _tenant1Id = _fixture.GenerateRandomTenantId();
        _tenant2Id = _fixture.GenerateRandomTenantId();
    }

    [Fact]
    public async Task GivenShowsWithDifferentVenueTenants_WhenQueryingShows_ThenOnlyShowsFromCurrentTenantVenuesAreReturned()
    {
        // Arrange - Create venues in different tenants
        VenueDto tenant1Venue, tenant2Venue;
        using (var context1 = CreateDbContext(_fixture.ConnectionString, _tenant1Id))
        {
            var tenantContext1 = new TestTenantContext(_tenant1Id);
            var venueService1 = new VenueService(context1, tenantContext1);
            tenant1Venue = await venueService1.CreateAsync(new CreateVenueDto
            {
                VenueGuid = Guid.NewGuid(),
                Name = $"Tenant1-Venue-{Guid.NewGuid().ToString()[..8]}",
                Address = "123 Test St",
                Latitude = 40.7128,
                Longitude = -74.0060,
                SeatingCapacity = 100,
                Description = "Test venue for tenant 1"
            });
        }

        using (var context2 = CreateDbContext(_fixture.ConnectionString, _tenant2Id))
        {
            var tenantContext2 = new TestTenantContext(_tenant2Id);
            var venueService2 = new VenueService(context2, tenantContext2);
            tenant2Venue = await venueService2.CreateAsync(new CreateVenueDto
            {
                VenueGuid = Guid.NewGuid(),
                Name = $"Tenant2-Venue-{Guid.NewGuid().ToString()[..8]}",
                Address = "456 Test Ave",
                Latitude = 34.0522,
                Longitude = -118.2437,
                SeatingCapacity = 200,
                Description = "Test venue for tenant 2"
            });
        }

        // Create a shared Act (could be in either tenant context)
        ActDto act;
        using (var context1 = CreateDbContext(_fixture.ConnectionString, _tenant1Id))
        {
            var tenantContext1 = new TestTenantContext(_tenant1Id);
            var actService = new ActService(context1, tenantContext1);
            act = await actService.CreateAsync(new CreateActDto
            {
                ActGuid = Guid.NewGuid(),
                Name = $"Shared-Act-{Guid.NewGuid().ToString()[..8]}"
            });
        }

        // Create shows in different tenant contexts using venues from different tenants
        ShowDto tenant1Show, tenant2Show;
        using (var context1 = CreateDbContext(_fixture.ConnectionString, _tenant1Id))
        {
            var tenantContext1 = new TestTenantContext(_tenant1Id);
            var showService1 = new ShowService(context1, tenantContext1);
            tenant1Show = await showService1.CreateAsync(new CreateShowDto
            {
                ShowGuid = Guid.NewGuid(),
                VenueGuid = tenant1Venue.VenueGuid,
                ActGuid = act.ActGuid,
                Date = DateTimeOffset.UtcNow.AddDays(30)
            });
        }

        using (var context2 = CreateDbContext(_fixture.ConnectionString, _tenant2Id))
        {
            var tenantContext2 = new TestTenantContext(_tenant2Id);
            var showService2 = new ShowService(context2, tenantContext2);
            tenant2Show = await showService2.CreateAsync(new CreateShowDto
            {
                ShowGuid = Guid.NewGuid(),
                VenueGuid = tenant2Venue.VenueGuid,
                ActGuid = act.ActGuid,
                Date = DateTimeOffset.UtcNow.AddDays(31)
            });
        }

        // Act & Assert - Tenant 1 should only see shows at Tenant 1 venues
        using (var context1 = CreateDbContext(_fixture.ConnectionString, _tenant1Id))
        {
            var tenantContext1 = new TestTenantContext(_tenant1Id);
            var showService1 = new ShowService(context1, tenantContext1);
            var tenant1Shows = (await showService1.GetAllAsync()).ToList();

            tenant1Shows.Should().Contain(s => s.ShowGuid == tenant1Show.ShowGuid);
            tenant1Shows.Should().NotContain(s => s.ShowGuid == tenant2Show.ShowGuid,
                "shows at venues from other tenants should be filtered out");
        }

        // Act & Assert - Tenant 2 should only see shows at Tenant 2 venues
        using (var context2 = CreateDbContext(_fixture.ConnectionString, _tenant2Id))
        {
            var tenantContext2 = new TestTenantContext(_tenant2Id);
            var showService2 = new ShowService(context2, tenantContext2);
            var tenant2Shows = (await showService2.GetAllAsync()).ToList();

            tenant2Shows.Should().Contain(s => s.ShowGuid == tenant2Show.ShowGuid);
            tenant2Shows.Should().NotContain(s => s.ShowGuid == tenant1Show.ShowGuid,
                "shows at venues from other tenants should be filtered out");
        }
    }

    [Fact]
    public async Task GivenShowsWithDifferentActTenants_WhenQueryingShows_ThenOnlyShowsFromCurrentTenantActsAreReturned()
    {
        // Arrange - Create acts in different tenants
        ActDto tenant1Act, tenant2Act;
        using (var context1 = CreateDbContext(_fixture.ConnectionString, _tenant1Id))
        {
            var tenantContext1 = new TestTenantContext(_tenant1Id);
            var actService1 = new ActService(context1, tenantContext1);
            tenant1Act = await actService1.CreateAsync(new CreateActDto
            {
                ActGuid = Guid.NewGuid(),
                Name = $"Tenant1-Act-{Guid.NewGuid().ToString()[..8]}"
            });
        }

        using (var context2 = CreateDbContext(_fixture.ConnectionString, _tenant2Id))
        {
            var tenantContext2 = new TestTenantContext(_tenant2Id);
            var actService2 = new ActService(context2, tenantContext2);
            tenant2Act = await actService2.CreateAsync(new CreateActDto
            {
                ActGuid = Guid.NewGuid(),
                Name = $"Tenant2-Act-{Guid.NewGuid().ToString()[..8]}"
            });
        }

        // Create a shared Venue (could be in either tenant context)
        VenueDto venue;
        using (var context1 = CreateDbContext(_fixture.ConnectionString, _tenant1Id))
        {
            var tenantContext1 = new TestTenantContext(_tenant1Id);
            var venueService = new VenueService(context1, tenantContext1);
            venue = await venueService.CreateAsync(new CreateVenueDto
            {
                VenueGuid = Guid.NewGuid(),
                Name = $"Shared-Venue-{Guid.NewGuid().ToString()[..8]}",
                Address = "123 Test St",
                Latitude = 40.7128,
                Longitude = -74.0060,
                SeatingCapacity = 100,
                Description = "Shared venue"
            });
        }

        // Create shows in different tenant contexts using acts from different tenants
        ShowDto tenant1Show, tenant2Show;
        using (var context1 = CreateDbContext(_fixture.ConnectionString, _tenant1Id))
        {
            var tenantContext1 = new TestTenantContext(_tenant1Id);
            var showService1 = new ShowService(context1, tenantContext1);
            tenant1Show = await showService1.CreateAsync(new CreateShowDto
            {
                ShowGuid = Guid.NewGuid(),
                VenueGuid = venue.VenueGuid,
                ActGuid = tenant1Act.ActGuid,
                Date = DateTimeOffset.UtcNow.AddDays(30)
            });
        }

        using (var context2 = CreateDbContext(_fixture.ConnectionString, _tenant2Id))
        {
            var tenantContext2 = new TestTenantContext(_tenant2Id);
            var showService2 = new ShowService(context2, tenantContext2);
            tenant2Show = await showService2.CreateAsync(new CreateShowDto
            {
                ShowGuid = Guid.NewGuid(),
                VenueGuid = venue.VenueGuid,
                ActGuid = tenant2Act.ActGuid,
                Date = DateTimeOffset.UtcNow.AddDays(31)
            });
        }

        // Act & Assert - Tenant 1 should only see shows with Tenant 1 acts
        using (var context1 = CreateDbContext(_fixture.ConnectionString, _tenant1Id))
        {
            var tenantContext1 = new TestTenantContext(_tenant1Id);
            var showService1 = new ShowService(context1, tenantContext1);
            var tenant1Shows = (await showService1.GetAllAsync()).ToList();

            tenant1Shows.Should().Contain(s => s.ShowGuid == tenant1Show.ShowGuid);
            tenant1Shows.Should().NotContain(s => s.ShowGuid == tenant2Show.ShowGuid,
                "shows with acts from other tenants should be filtered out");
        }

        // Act & Assert - Tenant 2 should only see shows with Tenant 2 acts
        using (var context2 = CreateDbContext(_fixture.ConnectionString, _tenant2Id))
        {
            var tenantContext2 = new TestTenantContext(_tenant2Id);
            var showService2 = new ShowService(context2, tenantContext2);
            var tenant2Shows = (await showService2.GetAllAsync()).ToList();

            tenant2Shows.Should().Contain(s => s.ShowGuid == tenant2Show.ShowGuid);
            tenant2Shows.Should().NotContain(s => s.ShowGuid == tenant1Show.ShowGuid,
                "shows with acts from other tenants should be filtered out");
        }
    }

    [Fact]
    public async Task GivenTicketSalesForShowsInDifferentTenants_WhenQueryingTicketSales_ThenOnlyTicketSalesForCurrentTenantShowsAreReturned()
    {
        // Arrange - Create complete show setup for two different tenants
        VenueDto tenant1Venue, tenant2Venue;
        ActDto tenant1Act, tenant2Act;
        ShowDto tenant1Show, tenant2Show;

        // Setup Tenant 1 venue, act, and show
        using (var context1 = CreateDbContext(_fixture.ConnectionString, _tenant1Id))
        {
            var tenantContext1 = new TestTenantContext(_tenant1Id);
            var venueService1 = new VenueService(context1, tenantContext1);
            tenant1Venue = await venueService1.CreateAsync(new CreateVenueDto
            {
                VenueGuid = Guid.NewGuid(),
                Name = $"T1-Venue-{Guid.NewGuid().ToString()[..8]}",
                Address = "123 Test St",
                Latitude = 40.7128,
                Longitude = -74.0060,
                SeatingCapacity = 100,
                Description = "Tenant 1 venue"
            });

            var actService1 = new ActService(context1, tenantContext1);
            tenant1Act = await actService1.CreateAsync(new CreateActDto
            {
                ActGuid = Guid.NewGuid(),
                Name = $"T1-Act-{Guid.NewGuid().ToString()[..8]}"
            });

            var showService1 = new ShowService(context1, tenantContext1);
            tenant1Show = await showService1.CreateAsync(new CreateShowDto
            {
                ShowGuid = Guid.NewGuid(),
                VenueGuid = tenant1Venue.VenueGuid,
                ActGuid = tenant1Act.ActGuid,
                Date = DateTimeOffset.UtcNow.AddDays(30)
            });
        }

        // Setup Tenant 2 venue, act, and show
        using (var context2 = CreateDbContext(_fixture.ConnectionString, _tenant2Id))
        {
            var tenantContext2 = new TestTenantContext(_tenant2Id);
            var venueService2 = new VenueService(context2, tenantContext2);
            tenant2Venue = await venueService2.CreateAsync(new CreateVenueDto
            {
                VenueGuid = Guid.NewGuid(),
                Name = $"T2-Venue-{Guid.NewGuid().ToString()[..8]}",
                Address = "456 Test Ave",
                Latitude = 34.0522,
                Longitude = -118.2437,
                SeatingCapacity = 200,
                Description = "Tenant 2 venue"
            });

            var actService2 = new ActService(context2, tenantContext2);
            tenant2Act = await actService2.CreateAsync(new CreateActDto
            {
                ActGuid = Guid.NewGuid(),
                Name = $"T2-Act-{Guid.NewGuid().ToString()[..8]}"
            });

            var showService2 = new ShowService(context2, tenantContext2);
            tenant2Show = await showService2.CreateAsync(new CreateShowDto
            {
                ShowGuid = Guid.NewGuid(),
                VenueGuid = tenant2Venue.VenueGuid,
                ActGuid = tenant2Act.ActGuid,
                Date = DateTimeOffset.UtcNow.AddDays(31)
            });
        }

        // Create ticket sales for both shows
        TicketSaleDto tenant1TicketSale, tenant2TicketSale;
        using (var context1 = CreateDbContext(_fixture.ConnectionString, _tenant1Id))
        {
            var tenantContext1 = new TestTenantContext(_tenant1Id);
            var ticketSaleService1 = new TicketSaleService(context1, tenantContext1);
            tenant1TicketSale = await ticketSaleService1.CreateAsync(new CreateTicketSaleDto
            {
                TicketSaleGuid = Guid.NewGuid(),
                ShowGuid = tenant1Show.ShowGuid,
                Quantity = 5
            });
        }

        using (var context2 = CreateDbContext(_fixture.ConnectionString, _tenant2Id))
        {
            var tenantContext2 = new TestTenantContext(_tenant2Id);
            var ticketSaleService2 = new TicketSaleService(context2, tenantContext2);
            tenant2TicketSale = await ticketSaleService2.CreateAsync(new CreateTicketSaleDto
            {
                TicketSaleGuid = Guid.NewGuid(),
                ShowGuid = tenant2Show.ShowGuid,
                Quantity = 10
            });
        }

        // Act & Assert - Tenant 1 should only see ticket sales for Tenant 1 shows
        using (var context1 = CreateDbContext(_fixture.ConnectionString, _tenant1Id))
        {
            var tenantContext1 = new TestTenantContext(_tenant1Id);
            var ticketSaleService1 = new TicketSaleService(context1, tenantContext1);
            var tenant1TicketSales = (await ticketSaleService1.GetAllAsync()).ToList();

            tenant1TicketSales.Should().Contain(ts => ts.TicketSaleGuid == tenant1TicketSale.TicketSaleGuid);
            tenant1TicketSales.Should().NotContain(ts => ts.TicketSaleGuid == tenant2TicketSale.TicketSaleGuid,
                "ticket sales for shows in other tenants should be filtered out");
        }

        // Act & Assert - Tenant 2 should only see ticket sales for Tenant 2 shows
        using (var context2 = CreateDbContext(_fixture.ConnectionString, _tenant2Id))
        {
            var tenantContext2 = new TestTenantContext(_tenant2Id);
            var ticketSaleService2 = new TicketSaleService(context2, tenantContext2);
            var tenant2TicketSales = (await ticketSaleService2.GetAllAsync()).ToList();

            tenant2TicketSales.Should().Contain(ts => ts.TicketSaleGuid == tenant2TicketSale.TicketSaleGuid);
            tenant2TicketSales.Should().NotContain(ts => ts.TicketSaleGuid == tenant1TicketSale.TicketSaleGuid,
                "ticket sales for shows in other tenants should be filtered out");
        }
    }

    [Fact]
    public async Task GivenShowInOneTenant_WhenQueriedFromAnotherTenant_ThenShowIsNotAccessible()
    {
        // Arrange - Create complete show setup in Tenant 1
        ShowDto tenant1Show;
        using (var context1 = CreateDbContext(_fixture.ConnectionString, _tenant1Id))
        {
            var tenantContext1 = new TestTenantContext(_tenant1Id);
            var venueService = new VenueService(context1, tenantContext1);
            var venue = await venueService.CreateAsync(new CreateVenueDto
            {
                VenueGuid = Guid.NewGuid(),
                Name = $"Venue-{Guid.NewGuid().ToString()[..8]}",
                Address = "123 Test St",
                Latitude = 40.7128,
                Longitude = -74.0060,
                SeatingCapacity = 100,
                Description = "Test venue"
            });

            var actService = new ActService(context1, tenantContext1);
            var act = await actService.CreateAsync(new CreateActDto
            {
                ActGuid = Guid.NewGuid(),
                Name = $"Act-{Guid.NewGuid().ToString()[..8]}"
            });

            var showService = new ShowService(context1, tenantContext1);
            tenant1Show = await showService.CreateAsync(new CreateShowDto
            {
                ShowGuid = Guid.NewGuid(),
                VenueGuid = venue.VenueGuid,
                ActGuid = act.ActGuid,
                Date = DateTimeOffset.UtcNow.AddDays(30)
            });
        }

        // Act - Try to access the show from Tenant 2 context
        using (var context2 = CreateDbContext(_fixture.ConnectionString, _tenant2Id))
        {
            var tenantContext2 = new TestTenantContext(_tenant2Id);
            var showService2 = new ShowService(context2, tenantContext2);
            var result = await showService2.GetByGuidAsync(tenant1Show.ShowGuid);

            // Assert - Show should not be accessible from a different tenant
            result.Should().BeNull("shows from other tenants should not be accessible");
        }
    }

    [Fact]
    public async Task GivenTicketSaleInOneTenant_WhenQueriedFromAnotherTenant_ThenTicketSaleIsNotAccessible()
    {
        // Arrange - Create complete setup in Tenant 1
        TicketSaleDto tenant1TicketSale;
        using (var context1 = CreateDbContext(_fixture.ConnectionString, _tenant1Id))
        {
            var tenantContext1 = new TestTenantContext(_tenant1Id);
            var venueService = new VenueService(context1, tenantContext1);
            var venue = await venueService.CreateAsync(new CreateVenueDto
            {
                VenueGuid = Guid.NewGuid(),
                Name = $"Venue-{Guid.NewGuid().ToString()[..8]}",
                Address = "123 Test St",
                Latitude = 40.7128,
                Longitude = -74.0060,
                SeatingCapacity = 100,
                Description = "Test venue"
            });

            var actService = new ActService(context1, tenantContext1);
            var act = await actService.CreateAsync(new CreateActDto
            {
                ActGuid = Guid.NewGuid(),
                Name = $"Act-{Guid.NewGuid().ToString()[..8]}"
            });

            var showService = new ShowService(context1, tenantContext1);
            var show = await showService.CreateAsync(new CreateShowDto
            {
                ShowGuid = Guid.NewGuid(),
                VenueGuid = venue.VenueGuid,
                ActGuid = act.ActGuid,
                Date = DateTimeOffset.UtcNow.AddDays(30)
            });

            var ticketSaleService = new TicketSaleService(context1, tenantContext1);
            tenant1TicketSale = await ticketSaleService.CreateAsync(new CreateTicketSaleDto
            {
                TicketSaleGuid = Guid.NewGuid(),
                ShowGuid = show.ShowGuid,
                Quantity = 5
            });
        }

        // Act - Try to access the ticket sale from Tenant 2 context
        using (var context2 = CreateDbContext(_fixture.ConnectionString, _tenant2Id))
        {
            var tenantContext2 = new TestTenantContext(_tenant2Id);
            var ticketSaleService2 = new TicketSaleService(context2, tenantContext2);
            var result = await ticketSaleService2.GetByGuidAsync(tenant1TicketSale.TicketSaleGuid);

            // Assert - Ticket sale should not be accessible from a different tenant
            result.Should().BeNull("ticket sales from other tenants should not be accessible");
        }
    }

    /// <summary>
    /// Creates and configures a GloboTicketDbContext for integration testing.
    /// </summary>
    /// <param name="connectionString">The database connection string.</param>
    /// <param name="tenantId">The tenant ID for the test context. Null for admin context.</param>
    /// <returns>A configured GloboTicketDbContext instance.</returns>
    private static GloboTicketDbContext CreateDbContext(string connectionString, int? tenantId)
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