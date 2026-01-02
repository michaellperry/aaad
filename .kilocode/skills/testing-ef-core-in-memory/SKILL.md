---
name: testing-ef-core-in-memory
description: Use this skill when writing unit tests that require an in-memory database context.
---

# Database Testing with EF Core In-Memory Provider

**CRITICAL: For all unit tests that require database operations, use the Entity Framework Core In-Memory Provider. Do NOT use mocks.**

## Setup Pattern

```csharp
using Microsoft.EntityFrameworkCore;

public class ServiceTests
{
    private GloboTicketDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<GloboTicketDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        // Create a mock or stub ITenantContext
        var tenantContext = new TestTenantContext { CurrentTenantId = 1 };

        return new GloboTicketDbContext(options, tenantContext);
    }
}
```

## Why In-Memory Provider?

- **Real EF Core behavior**: Tests run against actual DbContext logic
- **Query filter validation**: Multi-tenant filtering is tested with real queries
- **No mock setup complexity**: No need to mock DbSet, IQueryable, or async operations
- **Fast execution**: In-memory provider is optimized for testing
- **Relationship testing**: Navigate properties work as they would in production

## Test Tenant Context

Create a simple test implementation of `ITenantContext`:

```csharp
public class TestTenantContext : ITenantContext
{
    public int? CurrentTenantId { get; set; }
}
```

## Testing Multi-Tenancy

When testing multi-tenant entities:
1. Create entities with specific `TenantId` values
2. Set `TestTenantContext.CurrentTenantId` to filter queries
3. Verify that only entities matching the tenant are returned
4. Test with `CurrentTenantId = null` to verify admin/system queries

## Package Requirement

Ensure test projects reference:
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="9.0.0" />
```

