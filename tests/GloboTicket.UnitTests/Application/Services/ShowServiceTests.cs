using GloboTicket.Application.DTOs;
using GloboTicket.Application.Services;
using GloboTicket.Domain.Entities;
using GloboTicket.Infrastructure.Data;
using GloboTicket.UnitTests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace GloboTicket.UnitTests.Application.Services;

public class ShowServiceTests
{
    private static GloboTicketDbContext CreateContext(int? tenantId)
    {
        var options = new DbContextOptionsBuilder<GloboTicketDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var tenantContext = new TestTenantContext { CurrentTenantId = tenantId };
        var ctx = new GloboTicketDbContext(options, tenantContext);
        if (tenantId.HasValue)
        {
            ctx.Set<Tenant>().Add(new Tenant { Id = tenantId.Value, Slug = $"t{tenantId.Value}", Name = "Test Tenant", IsActive = true });
            ctx.SaveChanges();
        }
        return ctx;
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenTicketCountExceedsCapacity()
    {
        using var context = CreateContext(1);
        var act = new Act { ActGuid = Guid.NewGuid(), Name = "Act" };
        var venue = new Venue { VenueGuid = Guid.NewGuid(), Name = "Venue", SeatingCapacity = 100 };
        context.Set<Act>().Add(act);
        context.Set<Venue>().Add(venue);
        await context.SaveChangesAsync();

        var service = new ShowService(context, new TestTenantContext { CurrentTenantId = 1 });
        var dto = new CreateShowDto
        {
            ShowGuid = Guid.NewGuid(),
            VenueGuid = venue.VenueGuid,
            TicketCount = 200,
            StartTime = DateTimeOffset.UtcNow.AddHours(1)
        };

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(act.ActGuid, dto, CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_Succeeds_WithValidInput()
    {
        using var context = CreateContext(1);
        var act = new Act { ActGuid = Guid.NewGuid(), Name = "Act" };
        var venue = new Venue { VenueGuid = Guid.NewGuid(), Name = "Venue", SeatingCapacity = 100 };
        context.Set<Act>().Add(act);
        context.Set<Venue>().Add(venue);
        await context.SaveChangesAsync();

        var service = new ShowService(context, new TestTenantContext { CurrentTenantId = 1 });
        var dto = new CreateShowDto
        {
            ShowGuid = Guid.NewGuid(),
            VenueGuid = venue.VenueGuid,
            TicketCount = 50,
            StartTime = DateTimeOffset.UtcNow.AddHours(2)
        };

        var result = await service.CreateAsync(act.ActGuid, dto, CancellationToken.None);
        Assert.Equal(dto.ShowGuid, result.ShowGuid);
        Assert.Equal(venue.VenueGuid, result.VenueGuid);
        Assert.Equal(act.ActGuid, result.ActGuid);
    }

    [Fact]
    public async Task GetShowsByActGuid_Returns_CreatedShow()
    {
        using var context = CreateContext(1);
        var act = new Act { ActGuid = Guid.NewGuid(), Name = "Act" };
        var venue = new Venue { VenueGuid = Guid.NewGuid(), Name = "Venue", SeatingCapacity = 100 };
        var showGuid = Guid.NewGuid();
        context.Set<Act>().Add(act);
        context.Set<Venue>().Add(venue);
        context.Set<Show>().Add(new Show(act, venue) { ShowGuid = showGuid, TicketCount = 10, StartTime = DateTimeOffset.UtcNow.AddHours(3) });
        await context.SaveChangesAsync();

        var service = new ShowService(context, new TestTenantContext { CurrentTenantId = 1 });
        var results = await service.GetShowsByActGuidAsync(act.ActGuid, CancellationToken.None);

        Assert.Single(results);
        Assert.Equal(showGuid, results.First().ShowGuid);
    }
}
