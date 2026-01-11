using FluentAssertions;
using GloboTicket.Application.DTOs;
using GloboTicket.Application.Services;
using GloboTicket.Domain.Entities;
using GloboTicket.Infrastructure.Data;
using GloboTicket.UnitTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace GloboTicket.UnitTests.Application.Services;

public class TicketOfferServiceTests
{
    private static GloboTicketDbContext CreateContext(int? tenantId)
    {
        var options = new DbContextOptionsBuilder<GloboTicketDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
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
    public async Task GivenValidUpdate_WhenUpdateTicketOfferAsync_ThenTicketOfferIsUpdated()
    {
        // Arrange
        using var context = CreateContext(1);
        var (ticketOfferGuid, _) = await CreateTestTicketOfferAsync(context, 1, capacity: 1000, allocatedCount: 500);
        
        var dto = new UpdateTicketOfferDto
        {
            Name = "Updated Offer Name",
            Price = 75.00m,
            TicketCount = 600
        };

        // Act
        var service = new TicketOfferService(context, new TestTenantContext { CurrentTenantId = 1 });
        var result = await service.UpdateTicketOfferAsync(ticketOfferGuid, dto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Updated Offer Name");
        result.Price.Should().Be(75.00m);
        result.TicketCount.Should().Be(600);
        result.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task GivenTicketCountIncrease_WhenWithinCapacity_ThenUpdateSucceeds()
    {
        // Arrange
        using var context = CreateContext(1);
        var (ticketOfferGuid, _) = await CreateTestTicketOfferAsync(context, 1, capacity: 1000, allocatedCount: 500);
        
        var dto = new UpdateTicketOfferDto
        {
            Name = "General Admission",
            Price = 50.00m,
            TicketCount = 900 // Increase from 500 to 900 (400 increase, 100 available remaining)
        };

        // Act
        var service = new TicketOfferService(context, new TestTenantContext { CurrentTenantId = 1 });
        var result = await service.UpdateTicketOfferAsync(ticketOfferGuid, dto);

        // Assert
        result.Should().NotBeNull();
        result.TicketCount.Should().Be(900);
    }

    [Fact]
    public async Task GivenTicketCountDecrease_WhenUpdating_ThenUpdateSucceeds()
    {
        // Arrange
        using var context = CreateContext(1);
        var (ticketOfferGuid, _) = await CreateTestTicketOfferAsync(context, 1, capacity: 1000, allocatedCount: 500);
        
        var dto = new UpdateTicketOfferDto
        {
            Name = "General Admission",
            Price = 50.00m,
            TicketCount = 300 // Decrease from 500 to 300
        };

        // Act
        var service = new TicketOfferService(context, new TestTenantContext { CurrentTenantId = 1 });
        var result = await service.UpdateTicketOfferAsync(ticketOfferGuid, dto);

        // Assert
        result.Should().NotBeNull();
        result.TicketCount.Should().Be(300);
    }

    [Fact]
    public async Task GivenTicketCountExceedsCapacity_WhenUpdating_ThenThrowsArgumentException()
    {
        // Arrange
        using var context = CreateContext(1);
        var (ticketOfferGuid, _) = await CreateTestTicketOfferAsync(context, 1, capacity: 1000, allocatedCount: 500);
        
        var dto = new UpdateTicketOfferDto
        {
            Name = "General Admission",
            Price = 50.00m,
            TicketCount = 1100 // Exceeds capacity
        };

        // Act & Assert
        var service = new TicketOfferService(context, new TestTenantContext { CurrentTenantId = 1 });
        Func<Task> testAction = async () => await service.UpdateTicketOfferAsync(ticketOfferGuid, dto);

        await testAction.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*capacity*");
    }

    [Fact]
    public async Task GivenMultipleOffers_WhenUpdatingOneExceedsCapacity_ThenThrowsArgumentException()
    {
        // Arrange
        using var context = CreateContext(1);
        
        // Create show with 1000 capacity
        var (show, venue, performanceAct) = await CreateTestShowAsync(context, 1, capacity: 1000);
        
        // Create first offer with 600 tickets
        var offer1Guid = Guid.NewGuid();
        var offer1 = new TicketOffer(show, offer1Guid, "Early Bird", 40.00m, 600);
        context.TicketOffers.Add(offer1);
        
        // Create second offer with 300 tickets
        var offer2Guid = Guid.NewGuid();
        var offer2 = new TicketOffer(show, offer2Guid, "General Admission", 50.00m, 300);
        context.TicketOffers.Add(offer2);
        
        await context.SaveChangesAsync();
        
        // Try to update offer2 to 500 tickets (total would be 600 + 500 = 1100, exceeds 1000)
        var dto = new UpdateTicketOfferDto
        {
            Name = "General Admission",
            Price = 50.00m,
            TicketCount = 500
        };

        // Act & Assert
        var service = new TicketOfferService(context, new TestTenantContext { CurrentTenantId = 1 });
        Func<Task> act = async () => await service.UpdateTicketOfferAsync(offer2Guid, dto);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*400*"); // Should mention available capacity
    }

    [Fact]
    public async Task GivenNonExistentTicketOffer_WhenUpdating_ThenThrowsKeyNotFoundException()
    {
        // Arrange
        using var context = CreateContext(1);
        var nonExistentGuid = Guid.NewGuid();
        
        var dto = new UpdateTicketOfferDto
        {
            Name = "General Admission",
            Price = 50.00m,
            TicketCount = 500
        };

        // Act & Assert
        var service = new TicketOfferService(context, new TestTenantContext { CurrentTenantId = 1 });
        Func<Task> act = async () => await service.UpdateTicketOfferAsync(nonExistentGuid, dto);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task GivenDifferentTenantTicketOffer_WhenUpdating_ThenThrowsKeyNotFoundException()
    {
        // Arrange
        using var context1 = CreateContext(1);
        var (ticketOfferGuid, _) = await CreateTestTicketOfferAsync(context1, 1, capacity: 1000, allocatedCount: 500);
        
        var dto = new UpdateTicketOfferDto
        {
            Name = "Updated Name",
            Price = 75.00m,
            TicketCount = 600
        };

        // Act & Assert - Try to update from different tenant context
        using var context2 = CreateContext(2);
        var service = new TicketOfferService(context2, new TestTenantContext { CurrentTenantId = 2 });
        Func<Task> act = async () => await service.UpdateTicketOfferAsync(ticketOfferGuid, dto);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task GivenValidUpdate_WhenUpdating_ThenUpdatedAtIsSet()
    {
        // Arrange
        using var context = CreateContext(1);
        var (ticketOfferGuid, _) = await CreateTestTicketOfferAsync(context, 1, capacity: 1000, allocatedCount: 500);
        
        var beforeUpdate = DateTime.UtcNow;
        
        var dto = new UpdateTicketOfferDto
        {
            Name = "Updated Offer",
            Price = 60.00m,
            TicketCount = 550
        };

        // Act
        var service = new TicketOfferService(context, new TestTenantContext { CurrentTenantId = 1 });
        var result = await service.UpdateTicketOfferAsync(ticketOfferGuid, dto);
        
        var afterUpdate = DateTime.UtcNow;

        // Assert
        result.UpdatedAt.Should().NotBeNull();
        result.UpdatedAt.Should().BeOnOrAfter(beforeUpdate);
        result.UpdatedAt.Should().BeOnOrBefore(afterUpdate);
    }

    [Fact]
    public async Task GivenExactRemainingCapacity_WhenUpdating_ThenUpdateSucceeds()
    {
        // Arrange
        using var context = CreateContext(1);
        
        // Create show with 1000 capacity
        var (show, venue, performanceAct) = await CreateTestShowAsync(context, 1, capacity: 1000);
        
        // Create first offer with 600 tickets
        var offer1Guid = Guid.NewGuid();
        var offer1 = new TicketOffer(show, offer1Guid, "Early Bird", 40.00m, 600);
        context.TicketOffers.Add(offer1);
        
        // Create second offer with 200 tickets
        var offer2Guid = Guid.NewGuid();
        var offer2 = new TicketOffer(show, offer2Guid, "General Admission", 50.00m, 200);
        context.TicketOffers.Add(offer2);
        
        await context.SaveChangesAsync();
        
        // Update offer2 to use exact remaining capacity (400 = 1000 - 600)
        var dto = new UpdateTicketOfferDto
        {
            Name = "General Admission",
            Price = 50.00m,
            TicketCount = 400
        };

        // Act
        var service = new TicketOfferService(context, new TestTenantContext { CurrentTenantId = 1 });
        var result = await service.UpdateTicketOfferAsync(offer2Guid, dto);

        // Assert
        result.Should().NotBeNull();
        result.TicketCount.Should().Be(400);
    }

    #region Helper Methods

    /// <summary>
    /// Creates a test ticket offer with a show, venue, and act.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="tenantId">The tenant ID.</param>
    /// <param name="capacity">The show capacity.</param>
    /// <param name="allocatedCount">The number of tickets to allocate in the offer.</param>
    /// <returns>A tuple containing the ticket offer GUID and show GUID.</returns>
    private static async Task<(Guid ticketOfferGuid, Guid showGuid)> CreateTestTicketOfferAsync(
        GloboTicketDbContext context, 
        int tenantId, 
        int capacity, 
        int allocatedCount)
    {
        var (show, venue, performanceAct) = await CreateTestShowAsync(context, tenantId, capacity);
        
        var ticketOfferGuid = Guid.NewGuid();
        var ticketOffer = new TicketOffer(show, ticketOfferGuid, "General Admission", 50.00m, allocatedCount);
        context.TicketOffers.Add(ticketOffer);
        
        await context.SaveChangesAsync();
        
        return (ticketOfferGuid, show.ShowGuid);
    }

    /// <summary>
    /// Creates a test show with venue and act.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="tenantId">The tenant ID.</param>
    /// <param name="capacity">The show capacity.</param>
    /// <returns>A tuple containing the show, venue, and act.</returns>
    private static async Task<(Show show, Venue venue, Act performanceAct)> CreateTestShowAsync(
        GloboTicketDbContext context, 
        int tenantId, 
        int capacity)
    {
        var tenant = context.Tenants.FirstOrDefault(t => t.Id == tenantId);
        if (tenant == null)
        {
            tenant = new Tenant { Id = tenantId, Slug = $"t{tenantId}", Name = "Test Tenant", IsActive = true };
            context.Tenants.Add(tenant);
            await context.SaveChangesAsync();
        }

        var venue = new Venue
        {
            VenueGuid = Guid.NewGuid(),
            Name = "Test Venue",
            Address = "123 Test St",
            SeatingCapacity = capacity
        };
        
        var performanceAct = new Act
        {
            ActGuid = Guid.NewGuid(),
            Name = "Test Act"
        };
        
        context.Venues.Add(venue);
        context.Acts.Add(performanceAct);
        await context.SaveChangesAsync();
        
        var show = new Show(performanceAct, venue)
        {
            ShowGuid = Guid.NewGuid(),
            TicketCount = capacity,
            StartTime = DateTimeOffset.UtcNow.AddDays(30)
        };
        context.Shows.Add(show);
        await context.SaveChangesAsync();
        
        return (show, venue, performanceAct);
    }

    #endregion
}
