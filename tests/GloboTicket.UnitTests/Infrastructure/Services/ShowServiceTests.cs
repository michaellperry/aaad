using FluentAssertions;
using GloboTicket.Application.DTOs;
using GloboTicket.Domain.Entities;
using GloboTicket.Infrastructure.Data;
using GloboTicket.Infrastructure.Services;
using GloboTicket.UnitTests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace GloboTicket.UnitTests.Infrastructure.Services;

/// <summary>
/// Unit tests for the ShowService class.
/// Tests show operations including creation, retrieval, deletion, and nearby shows query.
/// Uses EF Core In-Memory provider for database operations.
/// </summary>
public class ShowServiceTests
{
    private const int DefaultTenantId = 1;
    private const int OtherTenantId = 2;

    #region Helper Methods

    /// <summary>
    /// Creates an in-memory database context for testing.
    /// Each test gets a unique database instance.
    /// </summary>
    private GloboTicketDbContext CreateInMemoryDbContext(int? tenantId = DefaultTenantId)
    {
        var options = new DbContextOptionsBuilder<GloboTicketDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var tenantContext = new TestTenantContext { CurrentTenantId = tenantId };
        return new GloboTicketDbContext(options, tenantContext);
    }

    /// <summary>
    /// Creates a test venue with default values.
    /// </summary>
    private Venue GivenVenue(
        Guid? venueGuid = null,
        string name = "Test Venue",
        int seatingCapacity = 1000,
        int tenantId = DefaultTenantId)
    {
        return new Venue
        {
            VenueGuid = venueGuid ?? Guid.NewGuid(),
            Name = name,
            SeatingCapacity = seatingCapacity,
            TenantId = tenantId,
            Description = "Test venue description"
        };
    }

    /// <summary>
    /// Creates a test act with default values.
    /// </summary>
    private Act GivenAct(
        Guid? actGuid = null,
        string name = "Test Act",
        int tenantId = DefaultTenantId)
    {
        return new Act
        {
            ActGuid = actGuid ?? Guid.NewGuid(),
            Name = name,
            TenantId = tenantId
        };
    }

    /// <summary>
    /// Creates a test show with required parent navigation properties.
    /// Parent entities (Act, Venue) are required parameters with no defaults.
    /// Scalar values have sensible defaults.
    /// </summary>
    private Show GivenShow(
        Act act,                              // Required - no default
        Venue venue,                          // Required - no default
        Guid? showGuid = null,                // Optional - has default
        int ticketCount = 500,                // Optional - has default
        DateTimeOffset? startTime = null)     // Optional - has default
    {
        return new Show
        {
            ShowGuid = showGuid ?? Guid.NewGuid(),
            Act = act,
            Venue = venue,
            TicketCount = ticketCount,
            StartTime = startTime ?? DateTimeOffset.UtcNow.AddDays(7)
        };
    }

    #endregion

    #region GetByGuidAsync Tests

    [Fact]
    public async Task GetByGuidAsync_WhenShowExists_ReturnsShowDto()
    {
        // Given: A show exists in the database
        await using var dbContext = CreateInMemoryDbContext();
        var venue = GivenVenue();
        var act = GivenAct();
        var showGuid = Guid.NewGuid();
        var show = GivenShow(showGuid: showGuid, act: act, venue: venue);

        dbContext.Venues.Add(venue);
        dbContext.Acts.Add(act);
        dbContext.Shows.Add(show);
        await dbContext.SaveChangesAsync();

        var service = new ShowService(dbContext);

        // When: Getting the show by GUID
        var result = await service.GetByGuidAsync(showGuid);

        // Then: The show DTO is returned with correct data
        result.Should().NotBeNull();
        result!.ShowGuid.Should().Be(showGuid);
        result.ActGuid.Should().Be(act.ActGuid);
        result.ActName.Should().Be(act.Name);
        result.VenueGuid.Should().Be(venue.VenueGuid);
        result.VenueName.Should().Be(venue.Name);
        result.VenueCapacity.Should().Be(venue.SeatingCapacity);
        result.TicketCount.Should().Be(show.TicketCount);
        result.StartTime.Should().Be(show.StartTime);
    }

    [Fact]
    public async Task GetByGuidAsync_WhenShowDoesNotExist_ReturnsNull()
    {
        // Given: An empty database
        await using var dbContext = CreateInMemoryDbContext();
        var service = new ShowService(dbContext);
        var nonExistentGuid = Guid.NewGuid();

        // When: Getting a show that doesn't exist
        var result = await service.GetByGuidAsync(nonExistentGuid);

        // Then: Null is returned
        result.Should().BeNull();
    }


    #endregion

    #region GetShowsByActGuidAsync Tests

    [Fact]
    public async Task GetShowsByActGuidAsync_WhenActHasShows_ReturnsAllShows()
    {
        // Given: An act with multiple shows
        await using var dbContext = CreateInMemoryDbContext();
        var venue = GivenVenue();
        var act = GivenAct();
        var show1 = GivenShow(act: act, venue: venue);
        var show2 = GivenShow(act: act, venue: venue);

        dbContext.Venues.Add(venue);
        dbContext.Acts.Add(act);
        dbContext.Shows.AddRange(show1, show2);
        await dbContext.SaveChangesAsync();

        var service = new ShowService(dbContext);

        // When: Getting shows by act GUID
        var result = await service.GetShowsByActGuidAsync(act.ActGuid);

        // Then: All shows for the act are returned
        var shows = result.ToList();
        shows.Should().HaveCount(2);
        shows.Should().AllSatisfy(s =>
        {
            s.ActGuid.Should().Be(act.ActGuid);
            s.ActName.Should().Be(act.Name);
        });
    }

    [Fact]
    public async Task GetShowsByActGuidAsync_WhenActHasNoShows_ReturnsEmptyCollection()
    {
        // Given: An act with no shows
        await using var dbContext = CreateInMemoryDbContext();
        var act = GivenAct();
        dbContext.Acts.Add(act);
        await dbContext.SaveChangesAsync();

        var service = new ShowService(dbContext);

        // When: Getting shows by act GUID
        var result = await service.GetShowsByActGuidAsync(act.ActGuid);

        // Then: An empty collection is returned
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetShowsByActGuidAsync_WhenActDoesNotExist_ThrowsKeyNotFoundException()
    {
        // Given: An empty database
        await using var dbContext = CreateInMemoryDbContext();
        var service = new ShowService(dbContext);
        var nonExistentActGuid = Guid.NewGuid();

        // When: Getting shows for a non-existent act
        var act = async () => await service.GetShowsByActGuidAsync(nonExistentActGuid);

        // Then: KeyNotFoundException is thrown
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Act with GUID {nonExistentActGuid} not found");
    }


    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_WithValidData_CreatesAndReturnsShow()
    {
        // Given: Valid act, venue, and show data
        await using var dbContext = CreateInMemoryDbContext();
        var venue = GivenVenue(seatingCapacity: 1000);
        var act = GivenAct();
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

        var service = new ShowService(dbContext);

        // When: Creating a show
        var result = await service.CreateAsync(act.ActGuid, createDto);

        // Then: The show is created and returned
        result.Should().NotBeNull();
        result.ShowGuid.Should().Be(createDto.ShowGuid);
        result.ActGuid.Should().Be(act.ActGuid);
        result.VenueGuid.Should().Be(venue.VenueGuid);
        result.TicketCount.Should().Be(createDto.TicketCount);
        result.StartTime.Should().Be(createDto.StartTime);

        // And: The show is persisted in the database
        var savedShow = await dbContext.Shows.FirstOrDefaultAsync(s => s.ShowGuid == createDto.ShowGuid);
        savedShow.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateAsync_WhenActDoesNotExist_ThrowsKeyNotFoundException()
    {
        // Given: A venue exists but the act does not
        await using var dbContext = CreateInMemoryDbContext();
        var venue = GivenVenue();
        dbContext.Venues.Add(venue);
        await dbContext.SaveChangesAsync();

        var createDto = new CreateShowDto
        {
            ShowGuid = Guid.NewGuid(),
            VenueGuid = venue.VenueGuid,
            TicketCount = 500,
            StartTime = DateTimeOffset.UtcNow.AddDays(7)
        };

        var service = new ShowService(dbContext);
        var nonExistentActGuid = Guid.NewGuid();

        // When: Creating a show with a non-existent act
        var create = async () => await service.CreateAsync(nonExistentActGuid, createDto);

        // Then: KeyNotFoundException is thrown
        await create.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Act with GUID {nonExistentActGuid} not found");
    }

    [Fact]
    public async Task CreateAsync_WhenVenueDoesNotExist_ThrowsKeyNotFoundException()
    {
        // Given: An act exists but the venue does not
        await using var dbContext = CreateInMemoryDbContext();
        var act = GivenAct();
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

        var service = new ShowService(dbContext);

        // When: Creating a show with a non-existent venue
        var create = async () => await service.CreateAsync(act.ActGuid, createDto);

        // Then: KeyNotFoundException is thrown
        await create.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Venue with GUID {nonExistentVenueGuid} not found");
    }

    [Fact]
    public async Task CreateAsync_WhenTicketCountExceedsVenueCapacity_ThrowsArgumentException()
    {
        // Given: A venue with limited capacity
        await using var dbContext = CreateInMemoryDbContext();
        var venue = GivenVenue(seatingCapacity: 100);
        var act = GivenAct();
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

        var service = new ShowService(dbContext);

        // When: Creating a show with ticket count exceeding capacity
        var create = async () => await service.CreateAsync(act.ActGuid, createDto);

        // Then: ArgumentException is thrown
        await create.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Ticket count cannot exceed venue capacity of 100*")
            .Where(ex => ex.ParamName == "TicketCount");
    }

    [Fact]
    public async Task CreateAsync_WhenStartTimeIsInPast_ThrowsArgumentException()
    {
        // Given: Valid act and venue
        await using var dbContext = CreateInMemoryDbContext();
        var venue = GivenVenue();
        var act = GivenAct();
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

        var service = new ShowService(dbContext);

        // When: Creating a show with a past start time
        var create = async () => await service.CreateAsync(act.ActGuid, createDto);

        // Then: ArgumentException is thrown
        await create.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Start time must be in the future*")
            .Where(ex => ex.ParamName == "StartTime");
    }


    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WhenShowExists_DeletesShowAndReturnsTrue()
    {
        // Given: A show exists in the database
        await using var dbContext = CreateInMemoryDbContext();
        var venue = GivenVenue();
        var act = GivenAct();
        var showGuid = Guid.NewGuid();
        var show = GivenShow(showGuid: showGuid, act: act, venue: venue);

        dbContext.Venues.Add(venue);
        dbContext.Acts.Add(act);
        dbContext.Shows.Add(show);
        await dbContext.SaveChangesAsync();

        var service = new ShowService(dbContext);

        // When: Deleting the show
        var result = await service.DeleteAsync(showGuid);

        // Then: True is returned
        result.Should().BeTrue();

        // And: The show is removed from the database
        var deletedShow = await dbContext.Shows.FirstOrDefaultAsync(s => s.ShowGuid == showGuid);
        deletedShow.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenShowDoesNotExist_ReturnsFalse()
    {
        // Given: An empty database
        await using var dbContext = CreateInMemoryDbContext();
        var service = new ShowService(dbContext);
        var nonExistentGuid = Guid.NewGuid();

        // When: Deleting a show that doesn't exist
        var result = await service.DeleteAsync(nonExistentGuid);

        // Then: False is returned
        result.Should().BeFalse();
    }


    #endregion

    #region GetNearbyShowsAsync Tests

    [Fact]
    public async Task GetNearbyShowsAsync_WhenShowsExistWithin48Hours_ReturnsNearbyShows()
    {
        // Given: A venue with shows within 48 hours
        await using var dbContext = CreateInMemoryDbContext();
        var venue = GivenVenue();
        var act1 = GivenAct(name: "Act 1");
        var act2 = GivenAct(name: "Act 2");

        var referenceTime = new DateTimeOffset(2026, 6, 15, 20, 0, 0, TimeSpan.Zero);
        var show1 = GivenShow(act: act1, venue: venue, startTime: referenceTime.AddHours(-24)); // 24 hours before
        var show2 = GivenShow(act: act2, venue: venue, startTime: referenceTime.AddHours(24));  // 24 hours after

        dbContext.Venues.Add(venue);
        dbContext.Acts.AddRange(act1, act2);
        dbContext.Shows.AddRange(show1, show2);
        await dbContext.SaveChangesAsync();

        var service = new ShowService(dbContext);

        // When: Getting nearby shows
        var result = await service.GetNearbyShowsAsync(venue.VenueGuid, referenceTime);

        // Then: Both shows are returned
        result.Should().NotBeNull();
        result.VenueGuid.Should().Be(venue.VenueGuid);
        result.VenueName.Should().Be(venue.Name);
        result.ReferenceTime.Should().Be(referenceTime);
        result.Shows.Should().HaveCount(2);
        result.Message.Should().Be("2 show(s) found within 48 hours");

        // And: Shows are ordered by start time
        result.Shows[0].StartTime.Should().Be(show1.StartTime);
        result.Shows[1].StartTime.Should().Be(show2.StartTime);
    }

    [Fact]
    public async Task GetNearbyShowsAsync_WhenNoShowsWithin48Hours_ReturnsEmptyList()
    {
        // Given: A venue with no shows within 48 hours
        await using var dbContext = CreateInMemoryDbContext();
        var venue = GivenVenue();
        var act = GivenAct();

        var referenceTime = new DateTimeOffset(2026, 6, 15, 20, 0, 0, TimeSpan.Zero);
        var show = GivenShow(act: act, venue: venue, startTime: referenceTime.AddHours(72)); // 72 hours after

        dbContext.Venues.Add(venue);
        dbContext.Acts.Add(act);
        dbContext.Shows.Add(show);
        await dbContext.SaveChangesAsync();

        var service = new ShowService(dbContext);

        // When: Getting nearby shows
        var result = await service.GetNearbyShowsAsync(venue.VenueGuid, referenceTime);

        // Then: Empty list is returned with appropriate message
        result.Should().NotBeNull();
        result.Shows.Should().BeEmpty();
        result.Message.Should().Be("No other shows scheduled at this venue within 48 hours");
    }

    [Fact]
    public async Task GetNearbyShowsAsync_WhenShowsAtExactBoundary_IncludesShows()
    {
        // Given: Shows at exactly 48 hours before and after
        await using var dbContext = CreateInMemoryDbContext();
        var venue = GivenVenue();
        var act1 = GivenAct(name: "Act 1");
        var act2 = GivenAct(name: "Act 2");

        var referenceTime = new DateTimeOffset(2026, 6, 15, 20, 0, 0, TimeSpan.Zero);
        var show1 = GivenShow(act: act1, venue: venue, startTime: referenceTime.AddHours(-48)); // Exactly 48 hours before
        var show2 = GivenShow(act: act2, venue: venue, startTime: referenceTime.AddHours(48));  // Exactly 48 hours after

        dbContext.Venues.Add(venue);
        dbContext.Acts.AddRange(act1, act2);
        dbContext.Shows.AddRange(show1, show2);
        await dbContext.SaveChangesAsync();

        var service = new ShowService(dbContext);

        // When: Getting nearby shows
        var result = await service.GetNearbyShowsAsync(venue.VenueGuid, referenceTime);

        // Then: Both boundary shows are included
        result.Shows.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetNearbyShowsAsync_WhenShowsBeyondBoundary_ExcludesShows()
    {
        // Given: Shows just beyond the 48-hour boundary
        await using var dbContext = CreateInMemoryDbContext();
        var venue = GivenVenue();
        var act1 = GivenAct(name: "Act 1");
        var act2 = GivenAct(name: "Act 2");

        var referenceTime = new DateTimeOffset(2026, 6, 15, 20, 0, 0, TimeSpan.Zero);
        var show1 = GivenShow(act: act1, venue: venue, startTime: referenceTime.AddHours(-49)); // 49 hours before
        var show2 = GivenShow(act: act2, venue: venue, startTime: referenceTime.AddHours(49));  // 49 hours after

        dbContext.Venues.Add(venue);
        dbContext.Acts.AddRange(act1, act2);
        dbContext.Shows.AddRange(show1, show2);
        await dbContext.SaveChangesAsync();

        var service = new ShowService(dbContext);

        // When: Getting nearby shows
        var result = await service.GetNearbyShowsAsync(venue.VenueGuid, referenceTime);

        // Then: No shows are returned
        result.Shows.Should().BeEmpty();
    }

    [Fact]
    public async Task GetNearbyShowsAsync_WhenVenueDoesNotExist_ThrowsKeyNotFoundException()
    {
        // Given: An empty database
        await using var dbContext = CreateInMemoryDbContext();
        var service = new ShowService(dbContext);
        var nonExistentVenueGuid = Guid.NewGuid();
        var referenceTime = DateTimeOffset.UtcNow;

        // When: Getting nearby shows for a non-existent venue
        var getNearby = async () => await service.GetNearbyShowsAsync(nonExistentVenueGuid, referenceTime);

        // Then: KeyNotFoundException is thrown
        await getNearby.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Venue with GUID {nonExistentVenueGuid} not found");
    }


    [Fact]
    public async Task GetNearbyShowsAsync_OnlyReturnsShowsForSpecifiedVenue()
    {
        // Given: Multiple venues with shows
        await using var dbContext = CreateInMemoryDbContext();
        var venue1 = GivenVenue(name: "Venue 1");
        var venue2 = GivenVenue(name: "Venue 2");
        var act = GivenAct();

        var referenceTime = new DateTimeOffset(2026, 6, 15, 20, 0, 0, TimeSpan.Zero);
        var show1 = GivenShow(act: act, venue: venue1, startTime: referenceTime.AddHours(12));
        var show2 = GivenShow(act: act, venue: venue2, startTime: referenceTime.AddHours(12));

        dbContext.Venues.AddRange(venue1, venue2);
        dbContext.Acts.Add(act);
        dbContext.Shows.AddRange(show1, show2);
        await dbContext.SaveChangesAsync();

        var service = new ShowService(dbContext);

        // When: Getting nearby shows for venue1
        var result = await service.GetNearbyShowsAsync(venue1.VenueGuid, referenceTime);

        // Then: Only shows for venue1 are returned
        result.Shows.Should().HaveCount(1);
        result.Shows[0].ShowGuid.Should().Be(show1.ShowGuid);
    }

    [Fact]
    public async Task GetNearbyShowsAsync_HandlesTimezonesCorrectly()
    {
        // Given: Shows with different timezone offsets
        await using var dbContext = CreateInMemoryDbContext();
        var venue = GivenVenue();
        var act = GivenAct();

        // Reference time in UTC
        var referenceTimeUtc = new DateTimeOffset(2026, 6, 15, 20, 0, 0, TimeSpan.Zero);
        
        // Show time with different offset but same UTC time
        var showTimeWithOffset = new DateTimeOffset(2026, 6, 15, 15, 0, 0, TimeSpan.FromHours(-5));
        var show = GivenShow(act: act, venue: venue, startTime: showTimeWithOffset);

        dbContext.Venues.Add(venue);
        dbContext.Acts.Add(act);
        dbContext.Shows.Add(show);
        await dbContext.SaveChangesAsync();

        var service = new ShowService(dbContext);

        // When: Getting nearby shows with UTC reference time
        var result = await service.GetNearbyShowsAsync(venue.VenueGuid, referenceTimeUtc);

        // Then: Show is included because UTC times match
        result.Shows.Should().HaveCount(1);
        result.Shows[0].StartTime.UtcDateTime.Should().Be(showTimeWithOffset.UtcDateTime);
    }

    #endregion
}
