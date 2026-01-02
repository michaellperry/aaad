using FluentAssertions;
using GloboTicket.Application.DTOs;
using GloboTicket.Domain.Entities;
using GloboTicket.Infrastructure.Data;
using GloboTicket.Infrastructure.Services;
using GloboTicket.UnitTests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace GloboTicket.UnitTests.Infrastructure.Services;

/// <summary>
/// Unit tests for ShowService using EF Core In-Memory Provider.
/// Tests all service methods including multi-tenant filtering, validation, and business logic.
/// </summary>
public class ShowServiceTests
{
    /// <summary>
    /// Creates an in-memory database context for testing with a pre-seeded tenant.
    /// Each test gets a unique database instance using Guid.NewGuid().
    /// </summary>
    /// <param name="tenantId">The tenant ID to set in the context. Defaults to 1.</param>
    /// <returns>A configured GloboTicketDbContext using in-memory provider.</returns>
    private GloboTicketDbContext CreateInMemoryDbContext(int? tenantId = 1)
    {
        var options = new DbContextOptionsBuilder<GloboTicketDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var tenantContext = new TestTenantContext { CurrentTenantId = tenantId };
        var dbContext = new GloboTicketDbContext(options, tenantContext);

        // Seed a tenant if tenantId is provided
        if (tenantId.HasValue)
        {
            var tenant = new Tenant
            {
                Id = tenantId.Value,
                TenantIdentifier = $"test-tenant-{tenantId}",
                Name = $"Test Tenant {tenantId}",
                Slug = $"test-{tenantId}"
            };
            dbContext.Tenants.Add(tenant);
            dbContext.SaveChanges();
        }

        return dbContext;
    }

    #region CreateShowAsync Tests

    [Fact]
    public async Task GivenValidData_WhenCreateShowAsync_ThenCreatesShowInDatabase()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var service = new ShowService(dbContext);

        var venue = new Venue
        {
            VenueGuid = Guid.NewGuid(),
            Name = "Test Venue",
            SeatingCapacity = 1000,
            Description = "Test Description"
        };
        var act = new Act { ActGuid = Guid.NewGuid(), Name = "Test Act" };

        dbContext.Venues.Add(venue);
        dbContext.Acts.Add(act);
        await dbContext.SaveChangesAsync();

        var createDto = new CreateShowDto
        {
            ShowGuid = Guid.NewGuid(),
            VenueGuid = venue.VenueGuid,
            TicketCount = 500,
            StartTime = DateTimeOffset.UtcNow.AddDays(7)
        };

        // Act
        var result = await service.CreateAsync(act.ActGuid, createDto);

        // Assert
        result.Should().NotBeNull();
        result.ShowGuid.Should().Be(createDto.ShowGuid);
        result.TicketCount.Should().Be(createDto.TicketCount);
        result.StartTime.Should().Be(createDto.StartTime);

        var showInDb = await dbContext.Shows.FirstOrDefaultAsync(s => s.ShowGuid == createDto.ShowGuid);
        showInDb.Should().NotBeNull();
    }

    [Fact]
    public async Task GivenValidData_WhenCreateShowAsync_ThenSetsCreatedAtTimestamp()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var service = new ShowService(dbContext);

        var venue = new Venue
        {
            VenueGuid = Guid.NewGuid(),
            Name = "Test Venue",
            SeatingCapacity = 1000,
            Description = "Test Description"
        };
        var act = new Act { ActGuid = Guid.NewGuid(), Name = "Test Act" };

        dbContext.Venues.Add(venue);
        dbContext.Acts.Add(act);
        await dbContext.SaveChangesAsync();

        var createDto = new CreateShowDto
        {
            ShowGuid = Guid.NewGuid(),
            VenueGuid = venue.VenueGuid,
            TicketCount = 500,
            StartTime = DateTimeOffset.UtcNow.AddDays(7)
        };

        var beforeCreate = DateTime.UtcNow;

        // Act
        var result = await service.CreateAsync(act.ActGuid, createDto);

        // Assert
        var afterCreate = DateTime.UtcNow;
        result.CreatedAt.Should().BeOnOrAfter(beforeCreate);
        result.CreatedAt.Should().BeOnOrBefore(afterCreate);
    }

    [Fact]
    public async Task GivenValidData_WhenCreateShowAsync_ThenAssociatesWithCorrectActAndVenue()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var service = new ShowService(dbContext);

        var venue = new Venue
        {
            VenueGuid = Guid.NewGuid(),
            Name = "Madison Square Garden",
            SeatingCapacity = 1000,
            Description = "Test Description"
        };
        var act = new Act { ActGuid = Guid.NewGuid(), Name = "The Rolling Stones" };

        dbContext.Venues.Add(venue);
        dbContext.Acts.Add(act);
        await dbContext.SaveChangesAsync();

        var createDto = new CreateShowDto
        {
            ShowGuid = Guid.NewGuid(),
            VenueGuid = venue.VenueGuid,
            TicketCount = 500,
            StartTime = DateTimeOffset.UtcNow.AddDays(7)
        };

        // Act
        var result = await service.CreateAsync(act.ActGuid, createDto);

        // Assert
        result.ActGuid.Should().Be(act.ActGuid);
        result.ActName.Should().Be(act.Name);
        result.VenueGuid.Should().Be(venue.VenueGuid);
        result.VenueName.Should().Be(venue.Name);
        result.VenueCapacity.Should().Be(venue.SeatingCapacity);
    }

    [Fact]
    public async Task GivenTicketCountExceedingCapacity_WhenCreateShowAsync_ThenThrowsArgumentException()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var service = new ShowService(dbContext);

        var venue = new Venue
        {
            VenueGuid = Guid.NewGuid(),
            Name = "Small Venue",
            SeatingCapacity = 100,
            Description = "Test Description"
        };
        var act = new Act { ActGuid = Guid.NewGuid(), Name = "Test Act" };

        dbContext.Venues.Add(venue);
        dbContext.Acts.Add(act);
        await dbContext.SaveChangesAsync();

        var createDto = new CreateShowDto
        {
            ShowGuid = Guid.NewGuid(),
            VenueGuid = venue.VenueGuid,
            TicketCount = 500, // Exceeds capacity of 100
            StartTime = DateTimeOffset.UtcNow.AddDays(7)
        };

        // Act
        var act_lambda = async () => await service.CreateAsync(act.ActGuid, createDto);

        // Assert
        await act_lambda.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*cannot exceed venue capacity of 100*");
    }

    [Fact]
    public async Task GivenPastStartTime_WhenCreateShowAsync_ThenThrowsArgumentException()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var service = new ShowService(dbContext);

        var venue = new Venue
        {
            VenueGuid = Guid.NewGuid(),
            Name = "Test Venue",
            SeatingCapacity = 1000,
            Description = "Test Description"
        };
        var act = new Act { ActGuid = Guid.NewGuid(), Name = "Test Act" };

        dbContext.Venues.Add(venue);
        dbContext.Acts.Add(act);
        await dbContext.SaveChangesAsync();

        var createDto = new CreateShowDto
        {
            ShowGuid = Guid.NewGuid(),
            VenueGuid = venue.VenueGuid,
            TicketCount = 500,
            StartTime = DateTimeOffset.UtcNow.AddDays(-1) // Past date
        };

        // Act
        var act_lambda = async () => await service.CreateAsync(act.ActGuid, createDto);

        // Assert
        await act_lambda.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*must be in the future*");
    }

    [Fact]
    public async Task GivenNonExistentVenue_WhenCreateShowAsync_ThenThrowsKeyNotFoundException()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var service = new ShowService(dbContext);

        var act = new Act { ActGuid = Guid.NewGuid(), Name = "Test Act" };
        dbContext.Acts.Add(act);
        await dbContext.SaveChangesAsync();

        var nonExistentVenueGuid = Guid.NewGuid();
        var createDto = new CreateShowDto
        {
            ShowGuid = Guid.NewGuid(),
            VenueGuid = nonExistentVenueGuid,
            TicketCount = 500,
            StartTime = DateTimeOffset.UtcNow.AddDays(7)
        };

        // Act
        var act_lambda = async () => await service.CreateAsync(act.ActGuid, createDto);

        // Assert
        await act_lambda.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{nonExistentVenueGuid}*");
    }

    [Fact]
    public async Task GivenNonExistentAct_WhenCreateShowAsync_ThenThrowsKeyNotFoundException()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var service = new ShowService(dbContext);

        var venue = new Venue
        {
            VenueGuid = Guid.NewGuid(),
            Name = "Test Venue",
            SeatingCapacity = 1000,
            Description = "Test Description"
        };
        dbContext.Venues.Add(venue);
        await dbContext.SaveChangesAsync();

        var nonExistentActGuid = Guid.NewGuid();
        var createDto = new CreateShowDto
        {
            ShowGuid = Guid.NewGuid(),
            VenueGuid = venue.VenueGuid,
            TicketCount = 500,
            StartTime = DateTimeOffset.UtcNow.AddDays(7)
        };

        // Act
        var act_lambda = async () => await service.CreateAsync(nonExistentActGuid, createDto);

        // Assert
        await act_lambda.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{nonExistentActGuid}*");
    }

    #endregion

    #region GetShowsByActGuidAsync Tests

    [Fact]
    public async Task GivenActWithShows_WhenGetShowsByActGuidAsync_ThenReturnsShowsForAct()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var service = new ShowService(dbContext);

        var venue = new Venue
        {
            VenueGuid = Guid.NewGuid(),
            Name = "Test Venue",
            SeatingCapacity = 1000,
            Description = "Test Description"
        };
        var act = new Act { ActGuid = Guid.NewGuid(), Name = "Test Act" };

        dbContext.Venues.Add(venue);
        dbContext.Acts.Add(act);
        await dbContext.SaveChangesAsync();

        var show1 = new Show
        {
            ShowGuid = Guid.NewGuid(),
            Act = act,
            ActId = act.Id,
            Venue = venue,
            VenueId = venue.Id,
            TicketCount = 500,
            StartTime = DateTimeOffset.UtcNow.AddDays(7)
        };
        var show2 = new Show
        {
            ShowGuid = Guid.NewGuid(),
            Act = act,
            ActId = act.Id,
            Venue = venue,
            VenueId = venue.Id,
            TicketCount = 300,
            StartTime = DateTimeOffset.UtcNow.AddDays(14)
        };

        dbContext.Shows.AddRange(show1, show2);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await service.GetShowsByActGuidAsync(act.ActGuid);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(s => s.ShowGuid == show1.ShowGuid);
        result.Should().Contain(s => s.ShowGuid == show2.ShowGuid);
    }

    [Fact]
    public async Task GivenActWithoutShows_WhenGetShowsByActGuidAsync_ThenReturnsEmptyList()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var service = new ShowService(dbContext);

        var act = new Act { ActGuid = Guid.NewGuid(), Name = "Test Act" };
        dbContext.Acts.Add(act);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await service.GetShowsByActGuidAsync(act.ActGuid);

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region GetByGuidAsync Tests

    [Fact]
    public async Task GivenExistingShow_WhenGetByGuidAsync_ThenReturnsShow()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var service = new ShowService(dbContext);

        var venue = new Venue
        {
            VenueGuid = Guid.NewGuid(),
            Name = "Test Venue",
            SeatingCapacity = 1000,
            Description = "Test Description"
        };
        var act = new Act { ActGuid = Guid.NewGuid(), Name = "Test Act" };

        dbContext.Venues.Add(venue);
        dbContext.Acts.Add(act);
        await dbContext.SaveChangesAsync();

        var showGuid = Guid.NewGuid();
        var show = new Show
        {
            ShowGuid = showGuid,
            Act = act,
            ActId = act.Id,
            Venue = venue,
            VenueId = venue.Id,
            TicketCount = 500,
            StartTime = DateTimeOffset.UtcNow.AddDays(7)
        };

        dbContext.Shows.Add(show);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await service.GetByGuidAsync(showGuid);

        // Assert
        result.Should().NotBeNull();
        result!.ShowGuid.Should().Be(showGuid);
        result.ActGuid.Should().Be(act.ActGuid);
        result.VenueGuid.Should().Be(venue.VenueGuid);
    }

    [Fact]
    public async Task GivenNonExistentShow_WhenGetByGuidAsync_ThenReturnsNull()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var service = new ShowService(dbContext);

        var nonExistentGuid = Guid.NewGuid();

        // Act
        var result = await service.GetByGuidAsync(nonExistentGuid);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetNearbyShowsAsync Tests

    [Fact]
    public async Task GivenShowsWithin48Hours_WhenGetNearbyShowsAsync_ThenReturnsShowsWithin48Hours()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var service = new ShowService(dbContext);

        var venue = new Venue
        {
            VenueGuid = Guid.NewGuid(),
            Name = "Test Venue",
            SeatingCapacity = 1000,
            Description = "Test Description"
        };
        var act1 = new Act { ActGuid = Guid.NewGuid(), Name = "Act 1" };
        var act2 = new Act { ActGuid = Guid.NewGuid(), Name = "Act 2" };

        dbContext.Venues.Add(venue);
        dbContext.Acts.AddRange(act1, act2);
        await dbContext.SaveChangesAsync();

        var referenceTime = new DateTimeOffset(2025, 7, 15, 20, 0, 0, TimeSpan.FromHours(-4));

        // Show 24 hours before (within 48 hours)
        var show1 = new Show
        {
            ShowGuid = Guid.NewGuid(),
            Act = act1,
            ActId = act1.Id,
            Venue = venue,
            VenueId = venue.Id,
            TicketCount = 500,
            StartTime = referenceTime.AddHours(-24)
        };

        // Show 30 hours after (within 48 hours)
        var show2 = new Show
        {
            ShowGuid = Guid.NewGuid(),
            Act = act2,
            ActId = act2.Id,
            Venue = venue,
            VenueId = venue.Id,
            TicketCount = 300,
            StartTime = referenceTime.AddHours(30)
        };

        dbContext.Shows.AddRange(show1, show2);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await service.GetNearbyShowsAsync(venue.VenueGuid, referenceTime);

        // Assert
        result.Should().NotBeNull();
        result.VenueGuid.Should().Be(venue.VenueGuid);
        result.Shows.Should().HaveCount(2);
        result.Shows.Should().Contain(s => s.ShowGuid == show1.ShowGuid);
        result.Shows.Should().Contain(s => s.ShowGuid == show2.ShowGuid);
    }

    [Fact]
    public async Task GivenShowsOutside48Hours_WhenGetNearbyShowsAsync_ThenExcludesShowsOutside48Hours()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var service = new ShowService(dbContext);

        var venue = new Venue
        {
            VenueGuid = Guid.NewGuid(),
            Name = "Test Venue",
            SeatingCapacity = 1000,
            Description = "Test Description"
        };
        var act1 = new Act { ActGuid = Guid.NewGuid(), Name = "Act 1" };
        var act2 = new Act { ActGuid = Guid.NewGuid(), Name = "Act 2" };
        var act3 = new Act { ActGuid = Guid.NewGuid(), Name = "Act 3" };

        dbContext.Venues.Add(venue);
        dbContext.Acts.AddRange(act1, act2, act3);
        await dbContext.SaveChangesAsync();

        var referenceTime = new DateTimeOffset(2025, 7, 15, 20, 0, 0, TimeSpan.FromHours(-4));

        // Show 49 hours before (outside window)
        var showTooEarly = new Show
        {
            ShowGuid = Guid.NewGuid(),
            Act = act1,
            ActId = act1.Id,
            Venue = venue,
            VenueId = venue.Id,
            TicketCount = 500,
            StartTime = referenceTime.AddHours(-49)
        };

        // Show 24 hours after (within window)
        var showInWindow = new Show
        {
            ShowGuid = Guid.NewGuid(),
            Act = act2,
            ActId = act2.Id,
            Venue = venue,
            VenueId = venue.Id,
            TicketCount = 300,
            StartTime = referenceTime.AddHours(24)
        };

        // Show 50 hours after (outside window)
        var showTooLate = new Show
        {
            ShowGuid = Guid.NewGuid(),
            Act = act3,
            ActId = act3.Id,
            Venue = venue,
            VenueId = venue.Id,
            TicketCount = 200,
            StartTime = referenceTime.AddHours(50)
        };

        dbContext.Shows.AddRange(showTooEarly, showInWindow, showTooLate);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await service.GetNearbyShowsAsync(venue.VenueGuid, referenceTime);

        // Assert
        result.Shows.Should().HaveCount(1);
        result.Shows.Should().Contain(s => s.ShowGuid == showInWindow.ShowGuid);
        result.Shows.Should().NotContain(s => s.ShowGuid == showTooEarly.ShowGuid);
        result.Shows.Should().NotContain(s => s.ShowGuid == showTooLate.ShowGuid);
    }

    [Fact]
    public async Task GivenMultipleShows_WhenGetNearbyShowsAsync_ThenSortsChronologically()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var service = new ShowService(dbContext);

        var venue = new Venue
        {
            VenueGuid = Guid.NewGuid(),
            Name = "Test Venue",
            SeatingCapacity = 1000,
            Description = "Test Description"
        };
        var act1 = new Act { ActGuid = Guid.NewGuid(), Name = "Act 1" };
        var act2 = new Act { ActGuid = Guid.NewGuid(), Name = "Act 2" };
        var act3 = new Act { ActGuid = Guid.NewGuid(), Name = "Act 3" };

        dbContext.Venues.Add(venue);
        dbContext.Acts.AddRange(act1, act2, act3);
        await dbContext.SaveChangesAsync();

        var referenceTime = new DateTimeOffset(2025, 7, 15, 20, 0, 0, TimeSpan.FromHours(-4));

        // Add shows out of chronological order
        var show2 = new Show
        {
            ShowGuid = Guid.NewGuid(),
            Act = act2,
            ActId = act2.Id,
            Venue = venue,
            VenueId = venue.Id,
            TicketCount = 300,
            StartTime = referenceTime.AddHours(10)
        };

        var show1 = new Show
        {
            ShowGuid = Guid.NewGuid(),
            Act = act1,
            ActId = act1.Id,
            Venue = venue,
            VenueId = venue.Id,
            TicketCount = 500,
            StartTime = referenceTime.AddHours(-10)
        };

        var show3 = new Show
        {
            ShowGuid = Guid.NewGuid(),
            Act = act3,
            ActId = act3.Id,
            Venue = venue,
            VenueId = venue.Id,
            TicketCount = 200,
            StartTime = referenceTime.AddHours(20)
        };

        dbContext.Shows.AddRange(show2, show1, show3);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await service.GetNearbyShowsAsync(venue.VenueGuid, referenceTime);

        // Assert
        result.Shows.Should().HaveCount(3);
        result.Shows.Should().BeInAscendingOrder(s => s.StartTime);
        result.Shows.ElementAt(0).ShowGuid.Should().Be(show1.ShowGuid);
        result.Shows.ElementAt(1).ShowGuid.Should().Be(show2.ShowGuid);
        result.Shows.ElementAt(2).ShowGuid.Should().Be(show3.ShowGuid);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task GivenExistingShow_WhenDeleteAsync_ThenDeletesShowAndReturnsTrue()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var service = new ShowService(dbContext);

        var venue = new Venue
        {
            VenueGuid = Guid.NewGuid(),
            Name = "Test Venue",
            SeatingCapacity = 1000,
            Description = "Test Description"
        };
        var act = new Act { ActGuid = Guid.NewGuid(), Name = "Test Act" };

        dbContext.Venues.Add(venue);
        dbContext.Acts.Add(act);
        await dbContext.SaveChangesAsync();

        var showGuid = Guid.NewGuid();
        var show = new Show
        {
            ShowGuid = showGuid,
            Act = act,
            ActId = act.Id,
            Venue = venue,
            VenueId = venue.Id,
            TicketCount = 500,
            StartTime = DateTimeOffset.UtcNow.AddDays(7)
        };

        dbContext.Shows.Add(show);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await service.DeleteAsync(showGuid);

        // Assert
        result.Should().BeTrue();

        var deletedShow = await dbContext.Shows.FirstOrDefaultAsync(s => s.ShowGuid == showGuid);
        deletedShow.Should().BeNull();
    }

    [Fact]
    public async Task GivenNonExistentShow_WhenDeleteAsync_ThenReturnsFalse()
    {
        // Arrange
        using var dbContext = CreateInMemoryDbContext();
        var service = new ShowService(dbContext);

        var nonExistentGuid = Guid.NewGuid();

        // Act
        var result = await service.DeleteAsync(nonExistentGuid);

        // Assert
        result.Should().BeFalse();
    }

    #endregion
}
