---
name: integration-testing-multi-tenant
description: Establishes patterns for creating test data in Entity Framework Core integration tests for multi-tenant applications. Use when writing integration tests that involve tenant isolation, foreign key relationships, or database context management with Testcontainers.
---

# Multi-Tenant Integration Testing Patterns

Patterns for creating test data in EF Core integration tests with multi-tenant architecture and proper foreign key relationship handling.

## Problem Statement

Integration tests in multi-tenant applications fail with foreign key constraint violations when test data creation methods span multiple database contexts. This occurs because Entity Framework Core contexts are independent units of work - entities saved in one context are not visible to another context until committed and reloaded.

## Core Principle: Single-Context Entity Graphs

**Rule**: Create complete entity relationship graphs within a **single `DbContext` instance** to ensure foreign key relationships are valid when `SaveChangesAsync()` is called.

### Entity Dependency Chain

In a multi-tenant architecture with the following relationships:

```
Tenant (root)
├── Venue (FK → Tenant)
├── Act (FK → Tenant)
└── Show (FK → Venue, FK → Act)
```

All related entities must be created in the same context.

## Patterns

### ❌ Anti-Pattern: Context Mismatch

**Problem**: Tenant created in one context, child entities in another

```csharp
// BROKEN: Tenant created via helper method with separate context
using (var context = _fixture.CreateDbContext(_connectionString, _tenantId))
{
    var tenant = await _fixture.CreateTestTenantAsync(_connectionString, _tenantId);
    // ↑ CreateTestTenantAsync() creates its own context internally
    
    var venue = new Venue 
    { 
        TenantId = tenant.Id,  // This FK references an entity context doesn't know about
        Name = "Test Venue"
    };
    context.Venues.Add(venue);
    await context.SaveChangesAsync(); // FK_Venues_Tenants_TenantId constraint violation!
}
```

**Why it fails**:
1. `CreateTestTenantAsync()` creates a new `DbContext`, saves the tenant, and disposes
2. The calling `context` has no knowledge of that tenant entity
3. When trying to insert `Venue`, SQL Server validates the FK and finds no matching `TenantId`

### ✅ Correct Pattern: Inline Entity Creation with Navigation Properties

**Solution**: Create all entities in the same context using navigation properties

```csharp
using var setupContext = _fixture.CreateDbContext(_connectionString, tenantId);

// 1. Create tenant
var tenant = new Tenant
{
    TenantIdentifier = $"test-tenant-{tenantId}-{uniqueId}",
    Name = $"Test Tenant {tenantId}",
    Slug = $"test-tenant-{tenantId}",
    IsActive = true
};
setupContext.Tenants.Add(tenant);
await setupContext.SaveChangesAsync();

// 2. Create venue using navigation property
var venue = new Venue
{
    Tenant = tenant,  // ✅ Navigation property - EF Core sets TenantId automatically
    VenueGuid = Guid.NewGuid(),
    Name = "Test Venue",
    Address = "123 Test St",
    SeatingCapacity = 1000
};
setupContext.Venues.Add(venue);
await setupContext.SaveChangesAsync();

// 3. Create act using navigation property
var act = new Act
{
    Tenant = tenant,  // ✅ Navigation property - EF Core sets TenantId automatically
    ActGuid = Guid.NewGuid(),
    Name = "Test Act"
};
setupContext.Acts.Add(act);
await setupContext.SaveChangesAsync();

// 4. Create show with FKs to venue and act
var show = new Show
{
    ShowGuid = Guid.NewGuid(),
    VenueId = venue.Id,    // Valid FK - same context
    ActId = act.Id,        // Valid FK - same context
    TicketCount = 500,
    StartTime = DateTimeOffset.UtcNow.AddDays(30)
};
setupContext.Shows.Add(show);
await setupContext.SaveChangesAsync();

return show.ShowGuid;
```

**Note**: The `TenantId` property has a private setter in `MultiTenantEntity` to enforce this pattern at compile time.

## Test Helper Method Design

### Bad: Helper Creates Own Context

```csharp
// ❌ ANTI-PATTERN
public async Task<Tenant> CreateTestTenantAsync(string connectionString, int tenantId)
{
    using var context = CreateDbContext(connectionString, null);  // New context!
    var tenant = new Tenant { ... };
    context.Tenants.Add(tenant);
    await context.SaveChangesAsync();
    return tenant;  // Detached entity returned to caller
}
```

### Good: Helper Uses Caller's Context

```csharp
// ✅ CORRECT PATTERN
public static Tenant CreateTenant(int seedId)
{
    return new Tenant
    {
        TenantIdentifier = $"test-tenant-{seedId}-{Guid.NewGuid().ToString()[..8]}",
        Name = $"Test Tenant {seedId}",
        Slug = $"test-tenant-{seedId}",
        IsActive = true
    };
}

// Usage in test:
using var context = _fixture.CreateDbContext(_connectionString, null);
var tenant = DatabaseHelpers.CreateTenant(_tenantId);
context.Tenants.Add(tenant);
await context.SaveChangesAsync();
// Now tenant.Id is populated and can be used for FKs
```

## Multi-Tenant Test Context Management

### Tenant ID vs Tenant Entity ID

**Important distinction**:
- **Test Tenant ID**: Random integer (1000-9999) for test isolation, generated by `GenerateRandomTenantId()`
- **Database Tenant ID**: Auto-generated primary key (`tenant.Id`) after `SaveChangesAsync()`

**✅ PREFERRED: Use navigation property** - EF Core handles the FK automatically:

```csharp
var testTenantId = _fixture.GenerateRandomTenantId();  // e.g., 4523 (for isolation)

using var context = _fixture.CreateDbContext(_connectionString, testTenantId);
var tenant = new Tenant { ... };
context.Tenants.Add(tenant);
await context.SaveChangesAsync();

// ✅ BEST: Use navigation property
var venue = new Venue
{
    Tenant = tenant,  // EF Core automatically sets TenantId to tenant.Id
    Name = "Venue"
};

// ❌ CANNOT DO: TenantId has private setter (compile error)
var badVenue = new Venue
{
    TenantId = tenant.Id,  // COMPILER ERROR! Property has private setter
    Name = "Bad Venue"
};

// ❌ Do NOT use test isolation ID
var worseVenue = new Venue
{
    Tenant = new Tenant { Id = testTenantId },  // WRONG! This is 4523, not a valid tenant
    Name = "Worse Venue"
};
```

**Why navigation properties are preferred:**
- EF Core automatically maintains FK consistency
- More expressive of domain relationships
- Enforced at compile time (TenantId has private setter)
- Prevents accidental use of wrong ID values

### Context Tenant Filtering

When creating a `DbContext` with a specific tenant ID, EF Core's global query filters apply:

```csharp
// Context with tenant filter - only sees tenant's data
using var tenantContext = _fixture.CreateDbContext(_connectionString, _testTenantId);
var shows = await tenantContext.Shows.ToListAsync();  // Filtered by tenant

// Context without filter - sees all data (admin mode)
using var adminContext = _fixture.CreateDbContext(_connectionString, null);
var allShows = await adminContext.Shows.ToListAsync();  // All tenants
```

**Pattern**: Create entities with `null` tenant context, verify with specific tenant context:

```csharp
// Setup: Create with admin context
using (var setupContext = _fixture.CreateDbContext(_connectionString, null))
{
    var tenant = CreateTenant();
    setupContext.Tenants.Add(tenant);
    await setupContext.SaveChangesAsync();
    
    var venue = new Venue { Tenant = tenant, ... };  // ✅ Navigation property
    setupContext.Venues.Add(venue);
    await setupContext.SaveChangesAsync();
}

// Act: Query with tenant context
using var testContext = _fixture.CreateDbContext(_connectionString, _testTenantId);
var service = new VenueService(testContext);
var result = await service.GetByGuidAsync(venueGuid);

// Result is filtered by global query filter
```

## Thread-Safe Database Migrations

When using Testcontainers with parallel test execution:

```csharp
private static readonly object _migrationLock = new object();

public GloboTicketDbContext CreateDbContext(string connectionString, int? tenantId)
{
    var options = new DbContextOptionsBuilder<GloboTicketDbContext>()
        .UseSqlServer(connectionString, sqlOptions => sqlOptions.UseNetTopologySuite())
        .Options;

    var tenantContext = new TestTenantContext(tenantId);
    var context = new GloboTicketDbContext(options, tenantContext);
    
    // Apply migrations in a thread-safe manner to avoid race conditions
    lock (_migrationLock)
    {
        context.Database.Migrate();  // ✅ Apply actual migrations
    }
    
    return context;
}
```

**Note**: `Database.Migrate()` is preferred over `EnsureCreated()` because it applies actual EF Core migrations, matching production behavior. `EnsureCreated()` bypasses migrations and directly creates the schema.

## Common Test Scenarios

### Cross-Tenant Isolation Test

```csharp
[Fact]
public async Task GetShow_FromOtherTenant_Returns404()
{
    var tenantAId = _fixture.GenerateRandomTenantId();
    var tenantBId = _fixture.GenerateRandomTenantId();
    
    // Create show in Tenant A
    Guid showGuid;
    using (var contextA = _fixture.CreateDbContext(_connectionString, null))
    {
        var tenantA = CreateTenant(tenantAId);
        contextA.Tenants.Add(tenantA);
        await contextA.SaveChangesAsync();
        
        var venue = new Venue { Tenant = tenantA, ... };  // ✅ Navigation property
        contextA.Venues.Add(venue);
        var act = new Act { Tenant = tenantA, ... };  // ✅ Navigation property
        contextA.Acts.Add(act);
        await contextA.SaveChangesAsync();
        
        var show = new Show { VenueId = venue.Id, ActId = act.Id, ... };
        contextA.Shows.Add(show);
        await contextA.SaveChangesAsync();
        showGuid = show.ShowGuid;
    }
    
    // Try to access from Tenant B
    using var contextB = _fixture.CreateDbContext(_connectionString, tenantBId);
    var service = new ShowService(contextB);
    var result = await service.GetByGuidAsync(showGuid);
    
    // Should not be visible due to tenant filtering
    result.Should().BeNull();
}
```

## Checklist for Integration Test Methods

When writing integration test helper methods:

- [ ] Create entities in a **single `DbContext`** per test scenario
- [ ] Use `null` tenant context for setup when creating cross-tenant test data
- [ ] Use **navigation properties** (e.g., `Tenant = tenant`) instead of foreign key properties (enforced by private setter)
- [ ] Always save parent entities before creating child entities with FKs
- [ ] Dispose contexts properly with `using` statements
- [ ] Apply `lock` around `Database.Migrate()` for thread safety
- [ ] Use `Database.Migrate()` instead of `Database.EnsureCreated()` for production-like behavior
- [ ] Return GUIDs or primitives from helper methods, not entity references
- [ ] Verify tenant isolation by querying with specific tenant contexts

## References

- Entity Framework Core: [DbContext Lifetime](https://learn.microsoft.com/ef/core/dbcontext-configuration/)
- Testcontainers: [SQL Server Container](https://dotnet.testcontainers.org/modules/mssql/)
- Multi-tenancy: [Global Query Filters](https://learn.microsoft.com/ef/core/querying/filters)
