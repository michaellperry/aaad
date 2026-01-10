using System;
using System.Threading;
using System.Threading.Tasks;
using GloboTicket.Application.DTOs;
using GloboTicket.Application.Services;
using GloboTicket.Domain.Entities;
using GloboTicket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GloboTicket.UnitTests.Application.Services;

public class TenantServiceTests
{
    private static GloboTicketDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<GloboTicketDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        // Use null tenant context for unit tests
        return new GloboTicketDbContext(options, new GloboTicket.UnitTests.Helpers.TestTenantContext { CurrentTenantId = null });
    }

    [Fact]
    public async Task CreateTenantAsync_SetsIsActive_True()
    {
        using var context = CreateContext();
        var service = new TenantService(context);
        var dto = new CreateTenantDto { Name = "Tenant A", Slug = "tenant-a", TenantIdentifier = "tid-a" };
        var result = await service.CreateTenantAsync(dto, CancellationToken.None);

        Assert.True(result.IsActive);
        Assert.Equal("tenant-a", result.Slug);
    }

    [Fact]
    public async Task GetTenantBySlugAsync_Returns_Tenant()
    {
        using var context = CreateContext();
        context.Set<Tenant>().Add(new Tenant { Name = "T1", Slug = "t1", TenantIdentifier = "id-t1", IsActive = true });
        await context.SaveChangesAsync();

        var service = new TenantService(context);
        var result = await service.GetTenantBySlugAsync("t1", CancellationToken.None);
        Assert.NotNull(result);
        Assert.Equal("T1", result!.Name);
    }
}
