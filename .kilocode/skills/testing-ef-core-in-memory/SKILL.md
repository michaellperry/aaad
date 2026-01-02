---
name: testing-ef-core-in-memory
description: Sets up Entity Framework Core in-memory database contexts for unit testing. Use when writing repository tests, testing multi-tenant queries, or validating EF Core query filters without mocking DbContext.
---

# Database Testing with EF Core In-Memory Provider

**CRITICAL: For all unit tests that require database operations, use the Entity Framework Core In-Memory Provider. Do NOT use mocks.**

> **Related Skill**: This skill provides the technical setup for EF Core in-memory testing. For test structure, naming conventions, and testing patterns (AAA, Given-When-Then, test data helpers), see the `tdd-testing-patterns` skill. These skills are designed to work together when writing service and repository tests.

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

## Related Patterns

When using this setup pattern, also follow the patterns from the `tdd-testing-patterns` skill:
- **Test Structure**: Use AAA (Arrange-Act-Assert) pattern with Given-When-Then comments
- **Test Naming**: Follow `MethodName_Scenario_ExpectedBehavior` convention
- **Test Data Helpers**: Use `Given` helper methods (e.g., `GivenVenue()`, `GivenAct()`) with default parameters
- **Service Testing**: See the `tdd-testing-patterns` skill's "Testing Application Services" section for complete examples

