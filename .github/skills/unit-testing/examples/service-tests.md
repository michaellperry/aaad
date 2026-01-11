# Service Testing Examples

Complete examples of testing application services with EF Core In-Memory database.

## ShowService Complete Example

```csharp
public class ShowServiceTests
{
    private const int DefaultTenantId = 1;

    private GloboTicketDbContext CreateInMemoryDbContext(int? tenantId = DefaultTenantId)
    {
        var options = new DbContextOptionsBuilder<GloboTicketDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var tenantContext = new TestTenantContext { CurrentTenantId = tenantId };
        return new GloboTicketDbContext(options, tenantContext);
    }

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
        result.TicketCount.Should().Be(createDto.TicketCount);

        // And: The show is persisted in the database
        var savedShow = await dbContext.Shows.FirstOrDefaultAsync(s => s.ShowGuid == createDto.ShowGuid);
        savedShow.Should().NotBeNull();
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
}

// Test data helpers for clarity and reuse
private Venue GivenVenue(
    Guid? venueGuid = null,
    string name = "Test Venue",
    int seatingCapacity = 1000)
{
    return new Venue
    {
        VenueGuid = venueGuid ?? Guid.NewGuid(),
        Name = name,
        SeatingCapacity = seatingCapacity,
        Description = "Test venue"
    };
}

private Act GivenAct(
    Guid? actGuid = null,
    string name = "Test Act")
{
    return new Act
    {
        ActGuid = actGuid ?? Guid.NewGuid(),
        Name = name
    };
}

private Show GivenShow(
    Act act,
    Venue venue,
    Guid? showGuid = null,
    int ticketCount = 500,
    DateTimeOffset? startTime = null)
{
    return new Show(act, venue)
    {
        ShowGuid = showGuid ?? Guid.NewGuid(),
        TicketCount = ticketCount,
        StartTime = startTime ?? DateTimeOffset.UtcNow.AddDays(7)
    };
}
```

## VenueService Example (DbContext + TestTenantContext)

```csharp
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
        await using var context = CreateContext(null);
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
        await using var context = CreateContext(1);
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
        updated.Should().NotBeNull();
        updated!.Name.Should().Be("New Name");
        updated.Address.Should().Be("New Address");
        updated.SeatingCapacity.Should().Be(200);
        updated.Description.Should().Be("New");
    }
}
```

## Mocking External Dependencies

```csharp
// ✅ Good - Mock an external dependency (pattern example)
public interface IEmailService
{
    Task SendConfirmationEmailAsync(string email, string subject);
}

private sealed class OrderService
{
    private readonly IEmailService _email;
    public OrderService(IEmailService email) => _email = email;

    public Task PlaceAsync(string email)
    {
        return _email.SendConfirmationEmailAsync(email, "Thanks");
    }
}

[Fact]
public async Task PlaceAsync_SendsConfirmationEmail()
{
    var emailService = Substitute.For<IEmailService>();
    var service = new OrderService(emailService);

    await service.PlaceAsync("user@test");

    await emailService.Received(1)
        .SendConfirmationEmailAsync("user@test", "Thanks");
}
```
