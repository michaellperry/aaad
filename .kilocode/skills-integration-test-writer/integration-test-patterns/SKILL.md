---
name: integration-test-patterns
description: Best practices for writing integration tests using Testcontainers and EF Core. Use when creating or modifying integration tests.
---

# Integration Test Patterns

## Infrastructure

### DatabaseFixture
Use the `DatabaseFixture` to manage the SQL Server container lifecycle. This fixture is shared across tests to reduce overhead.

```csharp
public class MyTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public MyTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }
}
```

### Context Management
Use the `CreateDbContext` helper pattern to create fresh contexts for Arrange and Act/Assert phases.

```csharp
private static GloboTicketDbContext CreateDbContext(string connectionString, int? tenantId)
{
    var options = new DbContextOptionsBuilder<GloboTicketDbContext>()
        .UseSqlServer(connectionString, sqlOptions => sqlOptions.UseNetTopologySuite())
        .Options;

    var tenantContext = new TestTenantContext(tenantId);
    var context = new GloboTicketDbContext(options, tenantContext);
    
    context.Database.Migrate();
    
    return context;
}
```

## Test Structure

### Fresh Context Pattern
Always use a separate context for verifying data persistence to ensure EF Core didn't just return cached entities.

```csharp
// Arrange
using (var setupContext = CreateDbContext(_fixture.ConnectionString, _tenantId))
{
    // ... create data ...
}

// Act & Assert
using (var context = CreateDbContext(_fixture.ConnectionString, _tenantId))
{
    // ... verify data ...
}
```

### Isolation
Use `_fixture.GenerateRandomTenantId()` and `Guid.NewGuid()` for properties like Slugs or Names to ensure tests do not collide in the shared container.

## Performance and Execution

For guidance on test parallelization, cleanup strategies, and CI/CD organization, see **[Performance](performance.md)**.

