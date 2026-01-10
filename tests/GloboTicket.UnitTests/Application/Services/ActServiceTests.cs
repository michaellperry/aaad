using GloboTicket.Application.DTOs;
using GloboTicket.Application.Services;
using GloboTicket.Domain.Entities;
using GloboTicket.Infrastructure.Data;
using GloboTicket.UnitTests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace GloboTicket.UnitTests.Application.Services;

public class ActServiceTests
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
        var service = new ActService(context, new TestTenantContext { CurrentTenantId = null });

        var dto = new CreateActDto { ActGuid = Guid.NewGuid(), Name = "The Luminaries" };

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync(dto, CancellationToken.None));
    }

    [Fact]
    public async Task GetAllAsync_Returns_SeededActs()
    {
        using var context = CreateContext(1);
        context.Set<Act>().Add(new Act { ActGuid = Guid.NewGuid(), Name = "Act A" });
        context.Set<Act>().Add(new Act { ActGuid = Guid.NewGuid(), Name = "Act B" });
        await context.SaveChangesAsync();

        var service = new ActService(context, new TestTenantContext { CurrentTenantId = 1 });
        var results = await service.GetAllAsync(CancellationToken.None);

        Assert.Equal(2, results.Count());
        Assert.Contains(results, a => a.Name == "Act A");
        Assert.Contains(results, a => a.Name == "Act B");
    }
}
