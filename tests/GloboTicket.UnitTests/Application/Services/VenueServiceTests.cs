using GloboTicket.Application.DTOs;
using GloboTicket.Application.Services;
using GloboTicket.Domain.Entities;
using GloboTicket.Domain.Services;
using GloboTicket.Infrastructure.Data;
using GloboTicket.UnitTests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace GloboTicket.UnitTests.Application.Services;

public class VenueServiceTests
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
    public async Task CreateAsync_Throws_WhenTenantMissing()
    {
        using var context = CreateContext(null);
        var service = new VenueService(context, new TestTenantContext { CurrentTenantId = null });

        var dto = new CreateVenueDto
        {
            VenueGuid = Guid.NewGuid(),
            Name = "Main Hall",
            Address = "123 Street",
            Latitude = 10.5,
            Longitude = -20.25,
            SeatingCapacity = 500,
            Description = "Nice place"
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync(dto, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateAsync_ModifiesFields()
    {
        using var context = CreateContext(1);
        var venue = new Venue
        {
            VenueGuid = Guid.NewGuid(),
            Name = "Old Name",
            Address = "Old Address",
            Location = GeographyService.CreatePoint(1, 1),
            SeatingCapacity = 100,
            Description = "Old"
        };
        context.Set<Venue>().Add(venue);
        await context.SaveChangesAsync();

        var service = new VenueService(context, new TestTenantContext { CurrentTenantId = 1 });
        var dto = new UpdateVenueDto
        {
            Name = "New Name",
            Address = "New Address",
            Latitude = 22,
            Longitude = 33,
            SeatingCapacity = 200,
            Description = "New"
        };

        var updated = await service.UpdateAsync(venue.Id, dto, CancellationToken.None);
        Assert.NotNull(updated);
        Assert.Equal("New Name", updated!.Name);
        Assert.Equal("New Address", updated.Address);
        Assert.Equal(200, updated.SeatingCapacity);
        Assert.Equal("New", updated.Description);
    }
}
