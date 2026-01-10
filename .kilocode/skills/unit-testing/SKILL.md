---
name: unit-testing
description: Provides unit testing patterns for .NET applications using EF Core In-Memory Provider, AAA structure, test data helpers, and TDD workflows. Use when writing unit tests for services, repositories, or domain logic.
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
private GloboTicketDbContext CreateInMemoryDbContext(int? tenantId = 1)
{
    var options = new DbContextOptionsBuilder<GloboTicketDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;

    var tenantContext = new TestTenantContext { CurrentTenantId = tenantId };
    return new GloboTicketDbContext(options, tenantContext);
}
```

**For detailed setup patterns**, see [`patterns/ef-core-in-memory-setup.md`](patterns/ef-core-in-memory-setup.md).

## Multi-Tenancy Testing

**Direct Tenant Filtering** (works in unit tests): Entities with direct `TenantId` properties

**Navigation Property Filtering** (integration tests only): EF Core In-Memory does NOT support query filters using navigation properties.

**For child entities filtered through navigation properties:**
- **Unit Tests**: Focus on business logic (validation, transformations, error handling)
- **Integration Tests**: Test tenant isolation with real database

**For detailed multi-tenancy patterns**, see [`patterns/ef-core-in-memory-setup.md`](patterns/ef-core-in-memory-setup.md).

## Test Data Helper Patterns

**CRITICAL**: Always initialize parent navigation properties, never foreign keys.

**❌ WRONG:**
```csharp
private Show GivenShow(int actId, int venueId) // Don't do this
```

**✅ CORRECT:**
```csharp
private Show GivenShow(Act act, Venue venue, int ticketCount = 500)
```

**Required vs. Optional Parameters:**
- **Required**: Parent navigation properties (no default, non-nullable)
- **Optional**: Scalar values (with defaults)

**For detailed patterns**, see [`patterns/test-data-helpers.md`](patterns/test-data-helpers.md).

## Mocking Strategy

- **✅ In-Memory Database**: All repository and service tests with data persistence
- **✅ Mocking**: External dependencies only (email, payment gateways, APIs)


## Best Practices

1. Use AAA Pattern with Given-When-Then comments
2. Follow `MethodName_Scenario_ExpectedBehavior` naming
3. Initialize navigation properties, never foreign keys
4. Make parent entities required parameters in Given methods
5. Use in-memory database for repository/service tests
6. Mock external dependencies only
7. Focus unit tests on business logic
8. Use integration tests for navigation property tenant isolation

## Additional Resources

- [`patterns/ef-core-in-memory-setup.md`](patterns/ef-core-in-memory-setup.md) - EF Core in-memory database setup and multi-tenancy testing
- [`patterns/test-data-helpers.md`](patterns/test-data-helpers.md) - Comprehensive test data helper patterns
- [`examples/service-tests.md`](examples/service-tests.md) - Complete service testing examples
- [`examples/domain-tests.md`](examples/domain-tests.md) - Entity and value object testing examples
