# EF Core In-Memory Database Setup

Detailed patterns for setting up and using EF Core In-Memory Provider in unit tests.

## Why In-Memory Provider?

- **Real EF Core behavior**: Tests run against actual DbContext logic
- **Query filter validation**: Multi-tenant filtering is tested with real queries
- **No mock setup complexity**: No need to mock DbSet, IQueryable, or async operations
- **Fast execution**: In-memory provider is optimized for testing
- **Relationship testing**: Navigation properties work as they would in production

## Setup Pattern

```csharp
using Microsoft.EntityFrameworkCore;
using GloboTicket.Infrastructure.Data;
using GloboTicket.UnitTests.Helpers;

public class ServiceTests
{
    private const int DefaultTenantId = 1;

    private GloboTicketDbContext CreateInMemoryDbContext(int? tenantId = DefaultTenantId)
    {
        var options = new DbContextOptionsBuilder<GloboTicketDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        // Create a test tenant context
        var tenantContext = new TestTenantContext { CurrentTenantId = tenantId };

        return new GloboTicketDbContext(options, tenantContext);
    }
}
```

## Test Tenant Context

Create a simple test implementation of `ITenantContext` in your test helpers:

```csharp
using GloboTicket.Infrastructure.MultiTenancy;

namespace GloboTicket.UnitTests.Helpers;

public class TestTenantContext : ITenantContext
{
    public int? CurrentTenantId { get; set; }
}
```

## Multi-Tenancy Testing Patterns

### Direct Tenant Filtering (Works in Unit Tests)

For entities with direct `TenantId` properties (e.g., `Venue`, `Act`, `Customer`):

```csharp
[Fact]
public async Task GetByGuidAsync_WhenVenueBelongsToDifferentTenant_ReturnsNull()
{
    // Given: A venue exists for a different tenant
    await using var dbContext = CreateInMemoryDbContext(tenantId: 1);
    var venue = GivenVenue(tenantId: 2); // Different tenant
    dbContext.Venues.Add(venue);
    await dbContext.SaveChangesAsync();

    var service = new VenueService(dbContext);

    // When: Getting the venue from tenant 1's context
    var result = await service.GetByGuidAsync(venue.VenueGuid);

    // Then: Null is returned due to tenant filtering
    result.Should().BeNull();
}
```

### Testing Admin/System Queries

Set `CurrentTenantId = null` to test admin or system-level queries that should bypass tenant filtering:

```csharp
[Fact]
public async Task GetAllVenues_WhenTenantIdIsNull_ReturnsAllVenues()
{
    // Given: Venues for multiple tenants
    await using var dbContext = CreateInMemoryDbContext(tenantId: null); // Admin context
    var venue1 = GivenVenue(tenantId: 1);
    var venue2 = GivenVenue(tenantId: 2);
    dbContext.Venues.AddRange(venue1, venue2);
    await dbContext.SaveChangesAsync();

    var service = new VenueService(dbContext);

    // When: Getting all venues with null tenant context
    var result = await service.GetAllAsync();

    // Then: All venues are returned regardless of tenant
    result.Should().HaveCount(2);
}
```

## Navigation Property Filtering Limitation

**IMPORTANT**: The EF Core In-Memory provider does NOT properly support query filters that use navigation properties.

### What Doesn't Work

```csharp
// This filter does NOT work correctly in in-memory tests
modelBuilder.Entity<Show>()
    .HasQueryFilter(s => _tenantContext.CurrentTenantId == null ||
                        s.Venue.TenantId == _tenantContext.CurrentTenantId);
```

### Why It Doesn't Work

The in-memory provider doesn't properly evaluate navigation property expressions in query filters. This is a known limitation of the provider.

### What to Do Instead

**For child entities filtered through navigation properties (e.g., `Show` filtered via `Venue.TenantId`):**

**Unit Tests - Focus on:**
- ✅ Entity not found scenarios
- ✅ Validation logic (e.g., ticket count <= venue capacity)
- ✅ Business rule enforcement (e.g., start time must be in future)
- ✅ Data mapping and transformations
- ✅ Query logic and filtering (non-tenant related)

**Integration Tests - Test:**
- ✅ Tenant isolation through navigation properties
- ✅ Complex query filters with relationships
- ✅ Multi-level navigation property filters

### Example: What NOT to Test in Unit Tests

```csharp
// DON'T write this test for child entities in unit tests
[Fact]
public async Task GetByGuidAsync_WhenShowBelongsToDifferentTenant_ReturnsNull()
{
    // This test will FAIL in in-memory tests because navigation property
    // filters don't work. Move this to integration tests instead.
    await using var dbContext = CreateInMemoryDbContext(tenantId: 1);
    var venue = GivenVenue(tenantId: 2);
    var act = GivenAct(tenantId: 2);
    var show = GivenShow(act: act, venue: venue);
    
    dbContext.Venues.Add(venue);
    dbContext.Acts.Add(act);
    dbContext.Shows.Add(show);
    await dbContext.SaveChangesAsync();

    var service = new ShowService(dbContext);
    var result = await service.GetByGuidAsync(show.ShowGuid);
    
    // This assertion will FAIL - the show will be returned even though
    // it belongs to a different tenant, because the navigation property
    // filter doesn't work in the in-memory provider
    result.Should().BeNull(); // FAILS
}
```

### Example: What TO Test in Unit Tests

```csharp
// DO write this test - it tests business logic, not tenant filtering
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
        .WithMessage("Ticket count cannot exceed venue capacity of 100*");
}
```

## Database Isolation Between Tests

Each test should get a unique database instance to ensure isolation:

```csharp
[Fact]
public async Task Test1()
{
    // Each test gets its own database
    await using var dbContext = CreateInMemoryDbContext();
    // ... test implementation
}

[Fact]
public async Task Test2()
{
    // This is a completely separate database from Test1
    await using var dbContext = CreateInMemoryDbContext();
    // ... test implementation
}
```

The `Guid.NewGuid().ToString()` database name ensures each test has an isolated database.

## Async Disposal

Use `await using` for proper async disposal of DbContext:

```csharp
[Fact]
public async Task MyTest()
{
    await using var dbContext = CreateInMemoryDbContext();
    // ... test implementation
    
    // DbContext is automatically disposed at the end of the method
}
```

## Package Requirements

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="10.0.0" />
```

## Common Patterns

### Constructor Setup (When Sharing Context)

```csharp
public class VenueServiceTests : IDisposable
{
    private readonly GloboTicketDbContext _context;
    private readonly VenueService _service;
    
    public VenueServiceTests()
    {
        var options = new DbContextOptionsBuilder<GloboTicketDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
            
        _context = new GloboTicketDbContext(options, new TestTenantContext { CurrentTenantId = 1 });
        _service = new VenueService(_context);
    }
    
    public void Dispose()
    {
        _context.Dispose();
    }
}
```

### Per-Test Setup (Recommended for Isolation)

```csharp
public class ShowServiceTests
{
    [Fact]
    public async Task MyTest()
    {
        // Each test creates its own context
        await using var dbContext = CreateInMemoryDbContext();
        var service = new ShowService(dbContext);
        
        // ... test implementation
    }
}
```

**Recommendation**: Use per-test setup for better isolation unless you have a specific reason to share context.
