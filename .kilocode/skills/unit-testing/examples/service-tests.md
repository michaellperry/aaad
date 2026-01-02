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
```

## VenueService Example with Mocking

```csharp
// ✅ Good - In-memory database with actual repository
public class VenueServiceTests
{
    private GloboTicketDbContext _context;
    private VenueRepository _repository;
    private VenueService _service;
    
    public VenueServiceTests()
    {
        var options = new DbContextOptionsBuilder<GloboTicketDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
            
        _context = new GloboTicketDbContext(options, new TestTenantContext { CurrentTenantId = 1 });
        _repository = new VenueRepository(_context);
        _service = new VenueService(_repository);
    }
    
    [Fact]
    public async Task CreateVenue_ValidData_CreatesVenueSuccessfully()
    {
        // Given: Valid venue data
        var createDto = new CreateVenueDto
        {
            Name = "Test Venue",
            Address = "Test Address"
        };
        
        // When: Creating a venue
        var result = await _service.CreateVenueAsync(createDto);
        
        // Then: Venue is created and persisted
        result.Should().NotBeNull();
        result.Name.Should().Be("Test Venue");
        
        var savedVenue = await _context.Venues.SingleOrDefaultAsync(v => v.Id == result.Id);
        savedVenue.Should().NotBeNull();
    }
}
```

## Mocking External Dependencies

```csharp
// ✅ Good - Mock external service dependency
public interface IEmailService
{
    Task SendConfirmationEmailAsync(string email, string subject);
}

[Fact]
public async Task CreateVenue_ValidData_SendsConfirmationEmail()
{
    // Given: Service with mocked email service
    var context = CreateInMemoryDbContext();
    var repository = new VenueRepository(context);
    var mockEmailService = Substitute.For<IEmailService>();
    var service = new VenueService(repository, mockEmailService);
    
    var createDto = new CreateVenueDto
    {
        Name = "Test Venue",
        Address = "Test Address"
    };
    
    // When: Creating a venue
    await service.CreateVenueAsync(createDto);
    
    // Then: Confirmation email is sent
    await mockEmailService.Received(1)
        .SendConfirmationEmailAsync(Arg.Any<string>(), Arg.Any<string>());
}
```

```csharp
// ❌ Bad - Mocking repository when in-memory database is available
[Fact]
public async Task CreateVenue_ValidData_CreatesVenue()
{
    var mockRepository = Substitute.For<IVenueRepository>();
    var service = new VenueService(mockRepository);
    // ... test implementation
}
```
