# Unit Testing Domain Logic

## Testing Entities and Value Objects

**Focus on testing business rules and invariants in domain objects.**

```csharp
public class VenueTests
{
    private Guid _tenantId;
    
    public VenueTests()
    {
        _tenantId = Guid.NewGuid();
    }
    
    [Fact]
    public void Venue_Deactivate_DeactivatesVenueAndAddsDomainEvent()
    {
        // Arrange
        var venue = GivenVenue();
        var originalEvents = DomainEvents.GetDomainEvents().ToList();
        
        // Act
        venue.Deactivate();
        
        // Assert - State changes
        venue.IsActive.Should().BeFalse();
        
        // Assert - Domain events
        var domainEvents = DomainEvents.GetDomainEvents();
        domainEvents.Should().Contain(e => e is VenueDeactivatedEvent);
        domainEvents.Count.Should().Be(originalEvents.Count + 1);
    }
    
    [Fact]
    public void Venue_AddAct_ValidAct_AddsActSuccessfully()
    {
        // Arrange
        var venue = GivenVenue();
        var actTitle = "Concert A";
        var eventDate = DateTime.UtcNow.AddDays(30);
        var ticketPrice = 50.00m;
        
        // Act
        venue.AddAct(actTitle, eventDate, ticketPrice);
        
        // Assert
        venue.Acts.Count.Should().Be(1);
        var act = venue.Acts.First();
        act.Title.Should().Be(actTitle);
        act.EventDate.Should().Be(eventDate);
        act.TicketPrice.Should().Be(ticketPrice);
    }
    
    [Fact]
    public void Venue_AddAct_InactiveVenue_ThrowsInvalidOperationException()
    {
        // Arrange
        var venue = GivenVenue();
        venue.Deactivate(); // Make venue inactive
        
        // Act & Assert
        var act = async () => await venue.AddAct("Concert A", DateTime.UtcNow.AddDays(30), 50.00m);
        
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*inactive venue*");
    }
    
    private Venue GivenVenue(string name = "Test Venue", bool isActive = true, Guid? tenantId = null)
    {
        return new Venue(name, CreateTestAddress(), tenantId ?? _tenantId)
        {
            IsActive = isActive
        };
    }
}
```

## Testing Value Objects

**Value objects should be tested for equality and immutability.**

```csharp
[Fact]
public void Address_Equals_SameValues_ReturnsTrue()
{
    // Arrange
    var address1 = new Address("123 Main St", "City", "State", "12345", "USA");
    var address2 = new Address("123 Main St", "City", "State", "12345", "USA");
    
    // Act & Assert
    address1.Should().Be(address2);
    (address1 == address2).Should().BeTrue();
}

[Fact]
public void Address_Equals_DifferentValues_ReturnsFalse()
{
    // Arrange
    var address1 = new Address("123 Main St", "City", "State", "12345", "USA");
    var address2 = new Address("456 Oak Ave", "City", "State", "12345", "USA");
    
    // Act & Assert
    address1.Should().NotBe(address2);
    (address1 == address2).Should().BeFalse();
}

[Fact]
public void Address_IsImmutable_ThrowsExceptionOnModification()
{
    // Arrange
    var address = new Address("123 Main St", "City", "State", "12345", "USA");
    
    // Act & Assert - Value objects should be immutable
    var act = () =>
    {
        var street = address.Street;
        // Attempting to modify would require reflection or specific API
    };
    
    act.Should().Throw<InvalidOperationException>();
}
```

## Testing Application Services

**Use in-memory database with actual repositories for all application service tests. This provides better coverage of database behavior and catches real EF Core query issues. See the [Mocking](mocking.md) pattern for details on when to use in-memory database vs. mocking.**

### Command Handler Tests
**Test application services that handle commands using in-memory database.**

```csharp
public class CreateVenueCommandHandlerTests
{
    private GloboTicketDbContext _context;
    private VenueRepository _repository;
    private CreateVenueCommandHandler _handler;
    private Guid _tenantId;
    
    public CreateVenueCommandHandlerTests()
    {
        _tenantId = Guid.NewGuid();
        var options = new DbContextOptionsBuilder<GloboTicketDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
            
        _context = new GloboTicketDbContext(options, MockTenantContext(_tenantId));
        _repository = new VenueRepository(_context);
        _handler = new CreateVenueCommandHandler(_repository, MockMapper.Object);
    }
    
    [Fact]
    public async Task CreateVenueCommandHandler_Handle_ValidCommand_ReturnsSuccess()
    {
        // Arrange
        var command = new CreateVenueCommand
        {
            Name = "Madison Square Garden",
            Address = "123 Broadway, New York, NY 10001",
            TenantId = _tenantId
        };
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBe(Guid.Empty);
        result.Name.Should().Be(command.Name);
        
        // Verify persistence
        var savedVenue = await _context.Venues.SingleOrDefaultAsync(v => v.Id == result.Id);
        savedVenue.Should().NotBeNull();
        savedVenue!.Name.Should().Be(command.Name);
    }
    
    [Fact]
    public async Task CreateVenueCommandHandler_DuplicateName_ThrowsValidationException()
    {
        // Arrange
        var existingVenue = GivenVenue(name: "Existing Venue");
        await _repository.AddAsync(existingVenue);
        await _context.SaveChangesAsync();
        
        var command = new CreateVenueCommand
        {
            Name = "Existing Venue",
            Address = "123 Broadway, New York, NY 10001",
            TenantId = _tenantId
        };
        
        // Act & Assert
        var act = async () => await _handler.Handle(command, CancellationToken.None);
        
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("*already exists*");
    }
    
    private Venue GivenVenue(string name = "Test Venue", Guid? tenantId = null)
    {
        return new Venue(name, CreateTestAddress(), tenantId ?? _tenantId);
    }
}
```

### Query Handler Tests
**Test read-only queries and data retrieval logic using in-memory database.**

```csharp
public class GetVenuesQueryHandlerTests
{
    private GloboTicketDbContext _context;
    private VenueRepository _repository;
    private GetVenuesQueryHandler _handler;
    private Guid _tenantId;
    
    public GetVenuesQueryHandlerTests()
    {
        _tenantId = Guid.NewGuid();
        var options = new DbContextOptionsBuilder<GloboTicketDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
            
        _context = new GloboTicketDbContext(options, MockTenantContext(_tenantId));
        _repository = new VenueRepository(_context);
        _handler = new GetVenuesQueryHandler(_repository);
    }
    
    [Fact]
    public async Task GetVenuesQueryHandler_Handle_ReturnsActiveVenues()
    {
        // Arrange
        var venue1 = GivenVenue(name: "Venue 1", isActive: true);
        var venue2 = GivenVenue(name: "Venue 2", isActive: true);
        var inactiveVenue = GivenVenue(name: "Inactive Venue", isActive: false);
        
        await _repository.AddAsync(venue1);
        await _repository.AddAsync(venue2);
        await _repository.AddAsync(inactiveVenue);
        await _context.SaveChangesAsync();
        
        var query = new GetVenuesQuery { TenantId = _tenantId };
        
        // Act
        var result = await _handler.Handle(query, CancellationToken.None);
        
        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(2);
        result.Should().Contain(v => v.Name == "Venue 1");
        result.Should().NotContain(v => v.Name == "Inactive Venue");
    }
    
    private Venue GivenVenue(string name = "Test Venue", bool isActive = true, Guid? tenantId = null)
    {
        return new Venue(name, CreateTestAddress(), tenantId ?? _tenantId)
        {
            IsActive = isActive
        };
    }
}
```

**Note**: The `Given` prefix in helper methods (e.g., `GivenVenue`) is a test data helper pattern. This is different from "Given-When-Then" comments used in test structure. See [Test Data Helpers](test-data-helpers.md) for details on the `Given` helper method pattern.

