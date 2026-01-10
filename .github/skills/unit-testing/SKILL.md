---
name: unit-testing
description: Unit testing patterns for .NET applications using EF Core In-Memory Provider, AAA structure, test data helpers, and TDD workflows. Use when writing unit tests for services, repositories, or domain logic.
---

# Unit Testing Patterns for .NET

Unit testing patterns for .NET applications covering test structure, EF Core in-memory database setup, test data helpers, and TDD workflows.

## TDD Philosophy

**TDD is about unit tests only.** Unit tests drive implementation using EF Core In-Memory Provider for fast, isolated testing.

**Integration tests follow implementation.** They use real databases to verify migrations, complex queries, and system integration.

## Test Structure

- **AAA Pattern**: Arrange-Act-Assert with Given-When-Then comments
- **Test Naming**: `MethodName_Scenario_ExpectedBehavior`
- **Given-When-Then Comments**: Describe test flow and intent

## EF Core In-Memory Database Setup

**CRITICAL: Use EF Core In-Memory Provider for all database operations. Do NOT mock repositories.**

```csharp
private GloboTicketDbContext CreateInMemoryDbContext(Guid? tenantId = null)
{
    var options = new DbContextOptionsBuilder<GloboTicketDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

    var tenantContext = new TestTenantContext 
    { 
        CurrentTenantId = tenantId ?? Guid.NewGuid() 
    };
    return new GloboTicketDbContext(options, tenantContext);
}

// Test base class
public abstract class TestBase
{
    protected GloboTicketDbContext _context;
    protected Guid _tenantId;
    
    [SetUp]
    public virtual void Setup()
    {
        _tenantId = Guid.NewGuid();
        _context = CreateInMemoryDbContext(_tenantId);
    }
    
    [TearDown]
    public virtual void TearDown()
    {
        _context?.Dispose();
    }
}
```

## Multi-Tenancy Testing

**Direct Tenant Filtering** (works in unit tests): Entities with direct `TenantId` properties

**Navigation Property Filtering** (integration tests only): EF Core In-Memory does NOT support query filters using navigation properties.

**For child entities filtered through navigation properties:**
- **Unit Tests**: Focus on business logic (validation, transformations, error handling)
- **Integration Tests**: Test tenant isolation with real database

## Test Data Helper Patterns

**CRITICAL**: Always initialize parent navigation properties, never foreign keys.

```csharp
// ❌ WRONG: Using foreign keys
private Show GivenShow(Guid actId, Guid venueId) // Don't do this

// ✅ CORRECT: Using navigation properties
private Show GivenShow(Act act, Venue venue, int ticketCount = 500)
{
    var show = new Show(
        eventDate: DateTime.UtcNow.AddDays(30),
        ticketPrice: 50.0m,
        ticketCount: ticketCount,
        act: act,
        venue: venue,
        tenantId: act.TenantId
    );
    
    _context.Shows.Add(show);
    _context.SaveChanges();
    return show;
}

// Required vs. Optional Parameters:
// Required: Parent navigation properties (no default, non-nullable)
// Optional: Scalar values (with defaults)
```

## Example Unit Tests

### Service Test Example
```csharp
[TestFixture]
public class VenueServiceTests : TestBase
{
    private VenueService _service;
    private VenueRepository _repository;
    
    [SetUp]
    public override void Setup()
    {
        base.Setup();
        _repository = new VenueRepository(_context);
        _service = new VenueService(_repository);
    }
    
    [Test]
    public async Task CreateVenue_ValidData_CreatesVenue()
    {
        // Arrange (Given)
        var createVenueDto = new CreateVenueDto
        {
            Name = "Test Arena",
            Address = new AddressDto 
            { 
                Street = "123 Test St",
                City = "Test City",
                State = "TS",
                PostalCode = "12345"
            }
        };
        
        // Act (When)
        var result = await _service.CreateVenueAsync(createVenueDto, _tenantId);
        
        // Assert (Then)
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Test Arena"));
        Assert.That(result.Address.Street, Is.EqualTo("123 Test St"));
        
        // Verify persistence
        var savedVenue = await _context.Venues
            .FirstOrDefaultAsync(v => v.Id == result.Id);
        Assert.That(savedVenue, Is.Not.Null);
        Assert.That(savedVenue.TenantId, Is.EqualTo(_tenantId));
    }
    
    [Test]
    public async Task GetVenue_DifferentTenant_ReturnsNull()
    {
        // Arrange (Given)
        var venue = GivenVenue("Test Venue");
        var differentTenantId = Guid.NewGuid();
        
        // Act (When)
        var result = await _service.GetVenueAsync(venue.Id, differentTenantId);
        
        // Assert (Then)
        Assert.That(result, Is.Null);
    }
    
    private Venue GivenVenue(string name, bool isActive = true)
    {
        var address = new Address("123 Test St", "Test City", "TS", "12345", "USA");
        var venue = new Venue(name, address, _tenantId);
        
        if (!isActive)
            venue.Deactivate();
            
        _context.Venues.Add(venue);
        _context.SaveChanges();
        return venue;
    }
}
```

### Domain Entity Test Example
```csharp
[TestFixture]
public class VenueTests
{
    [Test]
    public void Constructor_ValidData_CreatesVenue()
    {
        // Arrange (Given)
        var name = "Test Arena";
        var address = new Address("123 Test St", "Test City", "TS", "12345", "USA");
        var tenantId = Guid.NewGuid();
        
        // Act (When)
        var venue = new Venue(name, address, tenantId);
        
        // Assert (Then)
        Assert.That(venue.Name, Is.EqualTo(name));
        Assert.That(venue.Address, Is.EqualTo(address));
        Assert.That(venue.TenantId, Is.EqualTo(tenantId));
        Assert.That(venue.IsActive, Is.True);
        Assert.That(venue.Id, Is.Not.EqualTo(Guid.Empty));
    }
    
    [Test]
    public void AddAct_InactiveVenue_ThrowsException()
    {
        // Arrange (Given)
        var venue = CreateTestVenue();
        venue.Deactivate();
        
        // Act & Assert (When & Then)
        var ex = Assert.Throws<InvalidOperationException>(
            () => venue.AddAct("Test Act", DateTime.UtcNow.AddDays(30), 50.0m));
            
        Assert.That(ex.Message, Does.Contain("inactive venue"));
    }
    
    private Venue CreateTestVenue()
    {
        var address = new Address("123 Test St", "Test City", "TS", "12345", "USA");
        return new Venue("Test Venue", address, Guid.NewGuid());
    }
}
```

### Repository Test Example
```csharp
[TestFixture]
public class VenueRepositoryTests : TestBase
{
    private VenueRepository _repository;
    
    [SetUp]
    public override void Setup()
    {
        base.Setup();
        _repository = new VenueRepository(_context);
    }
    
    [Test]
    public async Task GetVenueAsync_ExistingVenue_ReturnsVenueDto()
    {
        // Arrange (Given)
        var venue = GivenVenue("Test Venue");
        
        // Act (When)
        var result = await _repository.GetVenueAsync(venue.Id, _tenantId);
        
        // Assert (Then)
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(venue.Id));
        Assert.That(result.Name, Is.EqualTo("Test Venue"));
    }
    
    [Test]
    public async Task GetVenueAsync_DifferentTenant_ReturnsNull()
    {
        // Arrange (Given)
        var venue = GivenVenue("Test Venue");
        var differentTenantId = Guid.NewGuid();
        
        // Act (When)
        var result = await _repository.GetVenueAsync(venue.Id, differentTenantId);
        
        // Assert (Then)
        Assert.That(result, Is.Null);
    }
    
    private Venue GivenVenue(string name)
    {
        var address = new Address("123 Test St", "Test City", "TS", "12345", "USA");
        var venue = new Venue(name, address, _tenantId);
        _context.Venues.Add(venue);
        _context.SaveChanges();
        return venue;
    }
}
```

## Mocking Strategy

- **✅ In-Memory Database**: All repository and service tests with data persistence
- **✅ Mocking**: External dependencies only (email, payment gateways, APIs)

```csharp
// ✅ Good: Mock external dependency
[Test]
public async Task SendTicketConfirmation_ValidOrder_SendsEmail()
{
    // Arrange
    var mockEmailService = new Mock<IEmailService>();
    var service = new OrderService(_repository, mockEmailService.Object);
    var order = GivenOrder();
    
    // Act
    await service.ConfirmOrderAsync(order.Id);
    
    // Assert
    mockEmailService.Verify(
        e => e.SendAsync(It.IsAny<TicketConfirmationEmail>()), 
        Times.Once);
}
```

## Best Practices

1. **Use AAA Pattern** with Given-When-Then comments
2. **Follow naming convention**: `MethodName_Scenario_ExpectedBehavior`
3. **Initialize navigation properties**, never foreign keys
4. **Make parent entities required parameters** in Given methods
5. **Use in-memory database** for repository/service tests
6. **Mock external dependencies only**
7. **Focus unit tests on business logic**
8. **Use integration tests** for navigation property tenant isolation
9. **Create unique database per test** to ensure isolation
10. **Always test multi-tenancy** when relevant

This approach ensures fast, reliable, and maintainable unit tests that provide confidence in the business logic implementation.