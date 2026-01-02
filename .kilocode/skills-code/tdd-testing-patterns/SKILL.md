---
name: tdd-testing-patterns
description: Test-Driven Development patterns and testing strategies for .NET applications
---

# Test-Driven Development Patterns

This skill provides comprehensive TDD patterns and testing strategies for .NET applications, supporting the Red-Green-Refactor cycle.

## Test Structure and Organization

### AAA Pattern (Arrange-Act-Assert)
**Structure every test using the AAA pattern for clarity and maintainability.**

```csharp
[Test]
public void Venue_Constructor_SetsPropertiesCorrectly()
{
    // Arrange - Setup test data and dependencies
    var name = "Madison Square Garden";
    var address = new Address("123 Broadway", "New York", "NY", "10001", "USA");
    var tenantId = Guid.NewGuid();
    
    // Act - Execute the method under test
    var venue = new Venue(name, address, tenantId);
    
    // Assert - Verify expected outcomes
    Assert.Multiple(() =>
    {
        Assert.That(venue.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(venue.Name, Is.EqualTo(name));
        Assert.That(venue.Address, Is.EqualTo(address));
        Assert.That(venue.TenantId, Is.EqualTo(tenantId));
        Assert.That(venue.IsActive, Is.True);
        Assert.That(venue.CreatedAt, Is.EqualTo(DateTime.UtcNow).Within(1).Seconds);
    });
}
```

### Test Naming Conventions
**Use descriptive test names that follow the pattern: MethodName_Scenario_ExpectedBehavior**

```csharp
// ✅ Good - Descriptive test names
[Test]
public void AddAct_ValidAct_AddsActToVenue()
{
    // Test implementation
}

[Test]
public void AddAct_InactiveVenue_ThrowsInvalidOperationException()
{
    // Test implementation
}

[Test]
public void VenueRepository_GetActiveVenues_ReturnsOnlyActiveVenues()
{
    // Test implementation
}

// ❌ Bad - Unclear test names
[Test]
public void Test1() { }
[Test]
public void TestVenue() { }
[Test]
public void AddTest() { }
```

### Given-When-Then Format
**Use Given-When-Then comments to make test intent crystal clear.**

```csharp
[Test]
public async Task VenueService_CreateVenue_ValidData_ReturnsCreatedVenue()
{
    // Given we have valid venue data
    var createVenueDto = new CreateVenueDto
    {
        Name = "The O2 Arena",
        Address = "Peninsula Square, London SE10 0DX",
        TenantId = _tenantId
    };
    
    // And our repository is empty (no existing venues)
    _venueRepository.Setup(r => r.ExistsByNameAsync(It.IsAny<string>(), It.IsAny<Guid>()))
        .ReturnsAsync(false);
    
    // When we create the venue
    var result = await _venueService.CreateVenueAsync(createVenueDto);
    
    // Then we should receive a valid venue DTO
    Assert.That(result, Is.Not.Null);
    Assert.That(result.Name, Is.EqualTo(createVenueDto.Name));
    Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
    
    // And the repository should have been called to save the venue
    _venueRepository.Verify(r => r.AddAsync(It.IsAny<Venue>()), Times.Once);
}
```

## Unit Testing Domain Logic

### Testing Entities and Value Objects
**Focus on testing business rules and invariants in domain objects.**

```csharp
[Test]
public void Venue_Deactivate_DeactivatesVenueAndAddsDomainEvent()
{
    // Arrange
    var venue = CreateValidVenue();
    var originalEvents = DomainEvents.GetDomainEvents().ToList();
    
    // Act
    venue.Deactivate();
    
    // Assert - State changes
    Assert.That(venue.IsActive, Is.False);
    
    // Assert - Domain events
    var domainEvents = DomainEvents.GetDomainEvents();
    Assert.That(domainEvents, Has.Some.TypeOf<VenueDeactivatedEvent>());
    Assert.That(domainEvents.Count, Is.EqualTo(originalEvents.Count + 1));
}

[Test]
public void Venue_AddAct_ValidAct_AddsActSuccessfully()
{
    // Arrange
    var venue = CreateValidVenue();
    var actTitle = "Concert A";
    var eventDate = DateTime.UtcNow.AddDays(30);
    var ticketPrice = 50.00m;
    
    // Act
    venue.AddAct(actTitle, eventDate, ticketPrice);
    
    // Assert
    Assert.That(venue.Acts.Count, Is.EqualTo(1));
    var act = venue.Acts.First();
    Assert.That(act.Title, Is.EqualTo(actTitle));
    Assert.That(act.EventDate, Is.EqualTo(eventDate));
    Assert.That(act.TicketPrice, Is.EqualTo(ticketPrice));
}

[Test]
public void Venue_AddAct_InactiveVenue_ThrowsInvalidOperationException()
{
    // Arrange
    var venue = CreateValidVenue();
    venue.Deactivate(); // Make venue inactive
    
    // Act & Assert
    var exception = Assert.ThrowsAsync<InvalidOperationException>(async () =>
    {
        await venue.AddAct("Concert A", DateTime.UtcNow.AddDays(30), 50.00m);
    });
    
    Assert.That(exception.Message, Does.Contain("inactive venue"));
}
```

### Testing Value Objects
**Value objects should be tested for equality and immutability.**

```csharp
[Test]
public void Address_Equals_SameValues_ReturnsTrue()
{
    // Arrange
    var address1 = new Address("123 Main St", "City", "State", "12345", "USA");
    var address2 = new Address("123 Main St", "City", "State", "12345", "USA");
    
    // Act & Assert
    Assert.That(address1, Is.EqualTo(address2));
    Assert.That(address1 == address2, Is.True);
}

[Test]
public void Address_Equals_DifferentValues_ReturnsFalse()
{
    // Arrange
    var address1 = new Address("123 Main St", "City", "State", "12345", "USA");
    var address2 = new Address("456 Oak Ave", "City", "State", "12345", "USA");
    
    // Act & Assert
    Assert.That(address1, Is.Not.EqualTo(address2));
    Assert.That(address1 == address2, Is.False);
}

[Test]
public void Address_IsImmutable_ThrowsExceptionOnModification()
{
    // Arrange
    var address = new Address("123 Main St", "City", "State", "12345", "USA");
    
    // Act & Assert - Value objects should be immutable
    var exception = Assert.Throws<InvalidOperationException>(() =>
    {
        var street = address.Street;
        // Attempting to modify would require reflection or specific API
    });
}
```

## Mocking and Dependency Management

### Mock External Dependencies
**Use interfaces for all dependencies and mock them in tests.**

```csharp
// ✅ Good - Interface-based design
public interface IVenueRepository
{
    Task<Venue?> GetByIdAsync(Guid id);
    Task AddAsync(Venue venue);
    Task<bool> ExistsByNameAsync(string name, Guid tenantId);
}

public class VenueService
{
    private readonly IVenueRepository _repository;
    
    public VenueService(IVenueRepository repository)
    {
        _repository = repository;
    }
}

// ✅ Good - Mocked dependencies in tests
[Test]
public async Task VenueService_CreateVenue_ValidData_CallsRepository()
{
    // Arrange
    var mockRepository = new Mock<IVenueRepository>();
    var service = new VenueService(mockRepository.Object);
    
    var createDto = new CreateVenueDto
    {
        Name = "Test Venue",
        Address = "Test Address",
        TenantId = _tenantId
    };
    
    mockRepository.Setup(r => r.ExistsByNameAsync(It.IsAny<string>(), It.IsAny<Guid>()))
        .ReturnsAsync(false);
    
    // Act
    var result = await service.CreateVenueAsync(createDto);
    
    // Assert
    mockRepository.Verify(r => r.AddAsync(It.IsAny<Venue>()), Times.Once);
}
```

### In-Memory Database for Repository Tests
**Use in-memory database for repository integration tests.**

```csharp
public class VenueRepositoryTests
{
    private GloboTicketDbContext _context;
    private VenueRepository _repository;
    private Guid _tenantId;
    
    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<GloboTicketDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
            
        _context = new GloboTicketDbContext(options, MockTenantContext(_tenantId));
        _repository = new VenueRepository(_context);
        _tenantId = Guid.NewGuid();
    }
    
    [Test]
    public async Task AddAsync_ValidVenue_AddsVenueSuccessfully()
    {
        // Arrange
        var venue = new Venue("Test Venue", CreateTestAddress(), _tenantId);
        
        // Act
        await _repository.AddAsync(venue);
        await _context.SaveChangesAsync();
        
        // Assert
        var savedVenue = await _context.Venues.FirstOrDefaultAsync(v => v.Id == venue.Id);
        Assert.That(savedVenue, Is.Not.Null);
        Assert.That(savedVenue!.Name, Is.EqualTo("Test Venue"));
    }
    
    [Test]
    public async Task GetByIdAsync_ExistingVenue_ReturnsVenue()
    {
        // Arrange
        var venue = new Venue("Test Venue", CreateTestAddress(), _tenantId);
        await _repository.AddAsync(venue);
        await _context.SaveChangesAsync();
        
        // Act
        var result = await _repository.GetByIdAsync(venue.Id);
        
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(venue.Id));
        Assert.That(result.Name, Is.EqualTo("Test Venue"));
    }
}
```

## Testing Application Services

### Command Handler Tests
**Test application services that handle commands and queries.**

```csharp
[Test]
public async Task CreateVenueCommandHandler_Handle_ValidCommand_ReturnsSuccess()
{
    // Arrange
    var mockRepository = new Mock<IVenueRepository>();
    var mockMapper = new Mock<IMapper>();
    var handler = new CreateVenueCommandHandler(mockRepository.Object, mockMapper.Object);
    
    var command = new CreateVenueCommand
    {
        Name = "Madison Square Garden",
        Address = "123 Broadway, New York, NY 10001",
        TenantId = _tenantId
    };
    
    mockRepository.Setup(r => r.ExistsByNameAsync(command.Name, command.TenantId))
        .ReturnsAsync(false);
    
    var expectedVenue = new Venue(command.Name, ParseAddress(command.Address), command.TenantId);
    var expectedDto = new VenueDto { Id = expectedVenue.Id, Name = expectedVenue.Name };
    
    mockMapper.Setup(m => m.Map<VenueDto>(It.IsAny<Venue>()))
        .Returns(expectedDto);
    
    // Act
    var result = await handler.Handle(command, CancellationToken.None);
    
    // Assert
    Assert.That(result, Is.Not.Null);
    Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
    Assert.That(result.Name, Is.EqualTo(command.Name));
    
    mockRepository.Verify(r => r.AddAsync(It.IsAny<Venue>()), Times.Once);
    mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
}

[Test]
public async Task CreateVenueCommandHandler_DuplicateName_ThrowsValidationException()
{
    // Arrange
    var mockRepository = new Mock<IVenueRepository>();
    var handler = new CreateVenueCommandHandler(mockRepository.Object, MockMapper.Object);
    
    var command = new CreateVenueCommand
    {
        Name = "Existing Venue",
        Address = "123 Broadway, New York, NY 10001",
        TenantId = _tenantId
    };
    
    mockRepository.Setup(r => r.ExistsByNameAsync(command.Name, command.TenantId))
        .ReturnsAsync(true);
    
    // Act & Assert
    var exception = Assert.ThrowsAsync<ValidationException>(async () =>
    {
        await handler.Handle(command, CancellationToken.None);
    });
    
    Assert.That(exception.Message, Does.Contain("already exists"));
}
```

### Query Handler Tests
**Test read-only queries and data retrieval logic.**

```csharp
[Test]
public async Task GetVenuesQueryHandler_Handle_ReturnsActiveVenues()
{
    // Arrange
    var mockRepository = new Mock<IVenueRepository>();
    var handler = new GetVenuesQueryHandler(mockRepository.Object);
    
    var venues = new List<Venue>
    {
        new Venue("Venue 1", CreateTestAddress(), _tenantId),
        new Venue("Venue 2", CreateTestAddress(), _tenantId)
    };
    
    mockRepository.Setup(r => r.GetActiveVenuesAsync(_tenantId))
        .ReturnsAsync(venues);
    
    var query = new GetVenuesQuery { TenantId = _tenantId };
    
    // Act
    var result = await handler.Handle(query, CancellationToken.None);
    
    // Assert
    Assert.That(result, Is.Not.Null);
    Assert.That(result.Count, Is.EqualTo(2));
    Assert.That(result.First().Name, Is.EqualTo("Venue 1"));
}
```

## Integration Testing

### API Endpoint Tests
**Test API controllers and HTTP endpoints.**

```csharp
[Test]
public async Task VenueEndpoints_CreateVenue_ValidRequest_ReturnsCreated()
{
    // Arrange
    var client = _factory.CreateClient();
    var request = new CreateVenueRequest
    {
        Name = "Madison Square Garden",
        Address = "123 Broadway, New York, NY 10001"
    };
    
    client.DefaultRequestHeaders.Add("X-Tenant-Id", _tenantId.ToString());
    
    // Act
    var response = await client.PostAsJsonAsync("/api/venues", request);
    
    // Assert
    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    
    var content = await response.Content.ReadAsStringAsync();
    var result = JsonSerializer.Deserialize<CreatedVenueResponse>(content);
    Assert.That(result, Is.Not.Null);
    Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
}

[Test]
public async Task VenueEndpoints_GetVenues_NoTenantHeader_ReturnsUnauthorized()
{
    // Arrange
    var client = _factory.CreateClient();
    
    // Act
    var response = await client.GetAsync("/api/venues");
    
    // Assert
    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
}
```

### Database Integration Tests
**Test with real database for integration scenarios.**

```csharp
[Collection("Database")]
public class VenueDatabaseTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    
    public VenueDatabaseTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public async Task VenueRepository_ComplexQuery_PerformsCorrectly()
    {
        // Arrange
        var tenantId = await _fixture.CreateTestTenantAsync();
        var venue = await _fixture.CreateTestVenueAsync(tenantId);
        var act = await _fixture.CreateTestActAsync(venue.Id, tenantId);
        
        // Act
        var result = await _fixture.VenueService
            .GetVenuesWithUpcomingActsAsync(tenantId, DateTime.UtcNow.AddDays(7));
        
        // Assert
        Assert.That(result, Is.Not.Empty);
        Assert.That(result.First().Acts, Has.Some.TypeOf<ActDto>());
    }
}
```

## Test Data Builders

### Object Mother Pattern
**Create reusable test data builders for complex objects.**

```csharp
public class VenueBuilder
{
    private string _name = "Test Venue";
    private Address _address = CreateTestAddress();
    private Guid _tenantId = Guid.NewGuid();
    private bool _isActive = true;
    
    public static VenueBuilder Create()
    {
        return new VenueBuilder();
    }
    
    public VenueBuilder WithName(string name)
    {
        _name = name;
        return this;
    }
    
    public VenueBuilder WithAddress(Address address)
    {
        _address = address;
        return this;
    }
    
    public VenueBuilder WithTenantId(Guid tenantId)
    {
        _tenantId = tenantId;
        return this;
    }
    
    public VenueBuilder Inactive()
    {
        _isActive = false;
        return this;
    }
    
    public Venue Build()
    {
        return new Venue(_name, _address, _tenantId) { IsActive = _isActive };
    }
}

// Usage in tests
[Test]
public void VenueService_CreateVenue_ValidData_ReturnsCreatedVenue()
{
    // Arrange
    var venue = VenueBuilder.Create()
        .WithName("Madison Square Garden")
        .WithTenantId(_tenantId)
        .Build();
    
    // Act & Assert
    // Test implementation
}
```

## Performance and Maintainability

### Test Parallelization
**Run tests in parallel for faster feedback.**

```csharp
// In test project file (.csproj)
<PropertyGroup>
    <ParallelizeTestExecution>true</ParallelizeTestExecution>
</PropertyGroup>

// In test class
[Parallelizable(ParallelScope.All)]
public class VenueServiceTests
{
    // Tests will run in parallel
}
```

### Test Cleanup
**Ensure proper cleanup between tests.**

```csharp
[Test]
public async Task TestCleanupExample()
{
    // Arrange
    var testVenue = await CreateTestDataAsync();
    
    try
    {
        // Act
        var result = await _service.ProcessVenueAsync(testVenue.Id);
        
        // Assert
        Assert.That(result.Success, Is.True);
    }
    finally
    {
        // Cleanup - ensure test data is removed
        await CleanupTestDataAsync(testVenue.Id);
    }
}
```

### Continuous Integration Testing
**Structure tests for CI/CD pipelines.**

```csharp
// Different test categories
[Trait("Category", "Unit")]
public class VenueDomainTests { /* Fast unit tests */ }

[Trait("Category", "Integration")]
public class VenueRepositoryTests { /* Database integration tests */ }

[Trait("Category", "Slow")]
public class VenuePerformanceTests { /* Performance tests */ }

// Run appropriate test categories in CI
dotnet test --filter "Category!=Slow"  // Fast CI builds
dotnet test --filter "Category=Slow"   // Nightly performance tests
```

Following these TDD patterns ensures comprehensive test coverage, maintainable test suites, and reliable feedback during development.